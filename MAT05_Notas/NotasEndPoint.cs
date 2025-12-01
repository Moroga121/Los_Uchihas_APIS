using MAT05_Notas.Entities;
using MAT05_Notas.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MAT05_Notas
{
    public static class NotasEndPoint
    {
      
        public static void MapRubroEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/cargardesglose").WithTags(nameof(DesgloseRubro));

            group.MapPost("/", async (
                [FromServices] Services.INotasService service,
                [FromBody] Entities.DesgloseRubro desglose, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
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
                    /// Validar curso exista

                    var cursoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD3CursosAvance3/api/curso/validar?nombre={Uri.EscapeDataString(desglose.nombre_curso)}");
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
                    var grupoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD4GruposAvance3/api/grupo/{desglose.nombre_grupo}");
                    grupoRequest.Headers.Add("access_token", accessToken);

                    var grupoResponse = await httpClient.SendAsync(grupoRequest);

                    if (grupoResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El grupo no existe" });
                    }

                    if (!grupoResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error consultando el grupo" });
                    }

                    desglose.Accion = "Crear";
                    var result = await service.Cargar_Desglose(desglose);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Cargar desglose de rubros",
                       descripcion: desglose,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("InsertarRubros")
            .WithOpenApi();
        }
        public static void MapNotaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/asignarnotarubro").WithTags(nameof(Notas));

            group.MapPost("/", async (
                [FromServices] Services.INotasService service,
                [FromBody] Entities.Notas nota, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
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
                    nota.Accion = "Crear";
                    var result = await service.Asignar_Actualizar_Nota(nota);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Crear nota",
                       descripcion: nota,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("InsertarNota")
            .WithOpenApi();

            group.MapPut("/", async (
                [FromServices] Services.INotasService service,
                [FromBody] Entities.Notas nota, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
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
                    nota.Accion = "Actualizar";
                    var result = await service.Asignar_Actualizar_Nota(nota);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Actualizar nota",
                       descripcion: nota,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("ActualizarNota")
            .WithOpenApi();
        }
        public static void MapDesgloseRubroEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/obtenerdesglose").WithTags(nameof(DesgloseRubro));

            group.MapGet("/{curso}/{grupo}", async (
            [FromServices] Services.INotasService service,
            string curso,
            string grupo, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
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
                            var result = await service.Obtener_Desglose_Por_ID(grupo, curso);
                            // Registrar intento exitoso en la bitacora del login
                            await service.RegistrarBitacoraAsync(
                               accion: "Obtener desglose por id",
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
        .WithName("GetRubrosById")
        .WithOpenApi();

        }
        public static void MapObtenerNotasGroup(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/obtenernotas").WithTags(nameof(DesgloseRubro));

            group.MapGet("/{numero_identificacion}/{curso}", async (
            [FromServices] Services.INotasService service,
            string numero_identificacion, string curso, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
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
                    var result = await service.Obtener_Notas_By_Id(numero_identificacion, curso);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener notas por id",
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
        .WithName("GetNotasById")
        .WithOpenApi();

        }
    }
}
