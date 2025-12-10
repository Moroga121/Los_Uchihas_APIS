using MAT01_Prematricula.Entities;
using MAT01_Prematricula.Services;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text.Json;
namespace MAT01_Prematricula
{
    public static class PrematriculaEndPoint
    {
        public static void MapPrematriculaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/Prematricula").WithTags(nameof(Prematricula));

            // Obtener todas las Prematriculas

            group.MapGet("/", async ([FromServices] Services.IPrematriculaService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }

                    var result = await service.Obtener_Todas_Prematriculas();
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener todos las Prematriculas",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("GetAllPrematriculas")
            .WithOpenApi();


            // Obtener Prematricula por Número de Identificación
            group.MapGet("/identificacion/{numero_identificacion}", async ([FromServices] Services.IPrematriculaService service, [FromRoute] string numero_identificacion, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
                {
                    try
                    {
                        // Validar token
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                        request.Headers.Add("access_token", accessToken);

                        var response = await httpClient.SendAsync(request);
                        if (!response.IsSuccessStatusCode)
                        {
                            return Results.Unauthorized();
                        }

                        // Llamar al servicio para obtener prematricula(s) por identificación
                        var prematriculas = await service.Obtener_Prematricula_Por_Identificacion(numero_identificacion);

                        if (prematriculas == null || !prematriculas.Any())
                        {
                            return Results.NotFound(new { mensaje = "No se encontraron prematriculas para la identificación proporcionada" });
                        }

                        return Results.Ok(prematriculas);
                    }
                    catch (Exception ex)
                    {
                        return Results.BadRequest(new { mensaje = ex.Message });
                    }
                })
                .WithName("GetPrematriculaPorIdentificacion")
                .WithOpenApi();



            // Obtener Prematricula por ID
            group.MapGet("/{Id_Prematricula}", async (
                [FromServices] Services.IPrematriculaService service,
                [FromRoute] string Id_Prematricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    var (prematricula, mensaje) = await service.Obtener_Prematricula_Por_ID(Id_Prematricula);
                    if (prematricula == null)
                    {
                        return Results.NotFound(new { mensaje = mensaje });
                    }
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener prematricula por id",
                       descripcion: prematricula,
                       accessToken: accessToken
                   );
                    return Results.Ok(prematricula);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
                .WithName("GetPrematriculaById")
                .WithOpenApi();


            #region "CRUD Actualizada"

            #region "CREAR PREMATRICULAS"

            // Crear Prematricula

            group.MapPost("/", async ([FromServices] Services.IPrematriculaService service, [FromBody] Entities.Prematricula prematricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    // Validar token 
                    var authRequest = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    authRequest.Headers.Add("access_token", accessToken);

                    var authResponse = await httpClient.SendAsync(authRequest);

                    if (!authResponse.IsSuccessStatusCode)
                    {
                        return Results.Unauthorized();
                    }

                    // Validar Estudiante

                    var estudianteRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/USR1Usuarios/usuario/id/{Uri.EscapeDataString(prematricula.numero_identificacion)}");
                    estudianteRequest.Headers.Add("access_token", accessToken);

                    var estudianteResponse = await httpClient.SendAsync(estudianteRequest);

                    
                    if (estudianteResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El estudiante no existe" });
                    }

                    
                    if (!estudianteResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error validando el estudiante en la API externa" });
                    }

                    

                    
                    var estudianteJson = await estudianteResponse.Content.ReadAsStringAsync();
                    var estudiante = JsonSerializer.Deserialize<Usuario>(estudianteJson);

                    
                    if (!string.Equals(estudiante!.Rol_Usuario, "Estudiante", StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(new { mensaje = "El usuario no tiene rol de estudiante" });
                    }


                   
                    // Validar Carrera
                    
                    var carreraRequest = new HttpRequestMessage(
                        HttpMethod.Get,
                        $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD2CarrerasAvance3/api/carrera/validar?nombre={prematricula.carrera}"
                    );
                    carreraRequest.Headers.Add("access_token", accessToken);

                    var carreraResponse = await httpClient.SendAsync(carreraRequest);

                    if (carreraResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "la carrera no existe" });
                    }

                    if (!carreraResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error validando la carrera en la API externa" });
                    }

                    // Validar curso y que coincida con la carrera

                    var cursoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD3CursosAvance3/api/curso/validar?nombre={Uri.EscapeDataString(prematricula.curso)}");
                    cursoRequest.Headers.Add("access_token", accessToken);
                    var cursoResponse = await httpClient.SendAsync(cursoRequest);

                    if (cursoResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "El curso no existe" });

                    }
                        if (!cursoResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando el curso" });

                    }
                        

                    var cursoJson = await cursoResponse.Content.ReadAsStringAsync();
                    var curso = JsonSerializer.Deserialize<Curso>(cursoJson);

                    var carreraJson = await carreraResponse.Content.ReadAsStringAsync();
                    var carrera = JsonSerializer.Deserialize<Carrera>(carreraJson);

                    // Validar que el curso pertenezca a la carrera
                    if (!string.Equals(curso!.ID_Carrera, carrera!.iD_Carrera, StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(new { mensaje = "El curso no pertenece a la carrera seleccionada" });
                    }



                    // Validar Periodo

                    var periodoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD5PeriodosAvance3/api/periodo/validar?id={Uri.EscapeDataString(prematricula.Id_Periodo)}");

                    periodoRequest.Headers.Add("access_token", accessToken);

                    var periodoResponse = await httpClient.SendAsync(periodoRequest);

                    if (periodoResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El periodo no existe" });
                    }

                    if (!periodoResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error validando el periodo en la API externa" });
                    }

                    // Leer el contenido y deserializar en la clase Periodo
                    var periodoJson = await periodoResponse.Content.ReadAsStringAsync();
                    var periodo = JsonSerializer.Deserialize<Periodo>(periodoJson);

                    if (periodo == null)
                    {
                        return Results.BadRequest(new { mensaje = "Periodo inválido" });
                    }

                    // Validar que el periodo sea futuro
                    if (periodo.Fecha_Inicio <= DateTime.Today)
                    {
                        return Results.BadRequest(new { mensaje = "Solo se pueden prematricular periodos futuros" });
                    }

                    prematricula.Accion = "Crear";
                    var result = await service.CRUDPrematricula(prematricula);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Crear prematricula",
                       descripcion: prematricula,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("RealizarPrematricula")
            .WithOpenApi();


            #endregion

            #region "ACTUALIZAR PREMATRICULA"

            // Actualizar Prematricula con validaciones
            group.MapPut("/", async ([FromServices] Services.IPrematriculaService service, [FromBody] Entities.Prematricula prematricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    
                    var authRequest = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    authRequest.Headers.Add("access_token", accessToken);

                    var authResponse = await httpClient.SendAsync(authRequest);
                    if (!authResponse.IsSuccessStatusCode)
                    {
                        return Results.Unauthorized();
                    }

                    
                    var estudianteRequest = new HttpRequestMessage(HttpMethod.Get,$"https://tiusr21pl.cuc-carrera-ti.ac.cr/USR1Usuarios/usuario/id/{Uri.EscapeDataString(prematricula.numero_identificacion)}");
                    
                    estudianteRequest.Headers.Add("access_token", accessToken);

                    var estudianteResponse = await httpClient.SendAsync(estudianteRequest);

                    if (estudianteResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El estudiante no existe" });
                    }
                        

                    if (!estudianteResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando el estudiante en la API externa" });

                    }
                        
                    var estudianteJson = await estudianteResponse.Content.ReadAsStringAsync();
                    var estudiante = JsonSerializer.Deserialize<Usuario>(estudianteJson);

                    if (!string.Equals(estudiante!.Rol_Usuario, "Estudiante", StringComparison.OrdinalIgnoreCase))
                    {

                        return Results.BadRequest(new { mensaje = "El usuario no tiene rol de estudiante" });

                    }
                        
                    var carreraRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD2CarrerasAvance3/api/carrera/validar?nombre={Uri.EscapeDataString(prematricula.carrera)}");
                    
                    carreraRequest.Headers.Add("access_token", accessToken);

                    var carreraResponse = await httpClient.SendAsync(carreraRequest);

                    if (carreraResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "La carrera no existe" });

                    }
                        
                    if (!carreraResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando la carrera en la API externa" });

                    }



                    var cursoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD3CursosAvance3/api/curso/validar?nombre={Uri.EscapeDataString(prematricula.curso)}");
                    cursoRequest.Headers.Add("access_token", accessToken);
                    var cursoResponse = await httpClient.SendAsync(cursoRequest);

                    if (cursoResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "El curso no existe" });

                    }
                    if (!cursoResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando el curso" });

                    }


                    var cursoJson = await cursoResponse.Content.ReadAsStringAsync();
                    var curso = JsonSerializer.Deserialize<Curso>(cursoJson);

                    var carreraJson = await carreraResponse.Content.ReadAsStringAsync();
                    var carrera = JsonSerializer.Deserialize<Carrera>(carreraJson);

                    // Validar que el curso pertenezca a la carrera
                    if (!string.Equals(curso!.ID_Carrera, carrera!.iD_Carrera, StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(new { mensaje = "El curso no pertenece a la carrera seleccionada" });
                    }



                    var periodoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD5PeriodosAvance3/api/periodo/validar?id={Uri.EscapeDataString(prematricula.Id_Periodo)}");

                    periodoRequest.Headers.Add("access_token", accessToken);

                    var periodoResponse = await httpClient.SendAsync(periodoRequest);

                    if (periodoResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El periodo no existe" });
                    }

                    if (!periodoResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error validando el periodo en la API externa" });
                    }

                    // Leer el contenido y deserializar en la clase Periodo
                    var periodoJson = await periodoResponse.Content.ReadAsStringAsync();
                    var periodo = JsonSerializer.Deserialize<Periodo>(periodoJson);

                    if (periodo == null)
                    {
                        return Results.BadRequest(new { mensaje = "Periodo inválido" });
                    }

                    // Validar que el periodo sea futuro
                    if (periodo.Fecha_Inicio <= DateTime.Today)
                    {
                        return Results.BadRequest(new { mensaje = "Solo se pueden prematricular periodos futuros" });
                    }



                    prematricula.Accion = "Actualizar";

                    var result = await service.CRUDPrematricula(prematricula);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Actualizar prematricula",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("UpdatePrematricula")
            .WithOpenApi();


            #endregion

            #region "ELIMINAR NO SE CAMBIÓ"

            // Eliminar Prematricula
            group.MapDelete("/{id}", async (
                int id,
                [FromServices] Services.IPrematriculaService service,
                [FromHeader(Name = "access_token")] string accessToken,
                HttpClient httpClient) =>
            {
                try
                {
                    // Validación del token
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Unauthorized();
                    }

                    // Crear el objeto para enviar al CRUD
                    var prematricula = new Entities.Prematricula
                    {
                        id_prematricula = id,
                        Accion = "Eliminar"
                    };

                    // Llamar al método del servicio
                    var result = await service.CRUDPrematricula(prematricula);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Eliminar prematricula",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            });

            #endregion

            #endregion


        }

    }
}
