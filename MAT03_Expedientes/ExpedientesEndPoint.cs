using MAT03_Expedientes.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MAT03_Expedientes
{
    public static class ExpedientesEndPoint
    {
        public static void MapExpedientesEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/expediente").WithTags(nameof(Expediente_Estudiantes));

            // Obtener todas las Expedientes
            group.MapGet("/", async ([FromServices] Services.IExpediente_EstudianteService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    var result = await service.Obtener_Todos_Expedientes();

                    // Registrar en bitacora

                    await service.RegistrarBitacoraAsync(
                        
                        accion: "Obtener todos los Expedientes",
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
            .WithName("GetAllExpedientes")
            .WithOpenApi();

            // Obtener Expediente por ID

            group.MapGet("/{numero_identificacion}", async (
                [FromServices] Services.IExpediente_EstudianteService service,
                [FromRoute] string numero_identificacion, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    var (expediente, mensaje) = await service.Obtener_Expediente_Por_ID(numero_identificacion);
                    if (expediente == null)
                    {
                        return Results.NotFound(new { mensaje = mensaje });
                    }

                    // Registrar en bitacora

                    await service.RegistrarBitacoraAsync(
                        
                        accion: "Obtener Expediente por ID",
                        descripcion: expediente,
                        accessToken: accessToken
                        );

                    return Results.Ok(expediente);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("GetExpedienteById")
            .WithOpenApi();

            #region "CRUD ACTUALIZADA"

            // Crear Expediente

            group.MapPost("/", async ([FromServices] Services.IExpediente_EstudianteService service, [FromBody] Entities.Expediente_Estudiantes expediente, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    // 1. Validar Token

                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);
                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                        

                    // 2. Consumir API de usuarios para verificar existencia

                    var usuarioRequest = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5000/usuario/id/{expediente.numero_identificacion}");
                    usuarioRequest.Headers.Add("access_token", accessToken);

                    var usuarioResponse = await httpClient.SendAsync(usuarioRequest);

                    if (!usuarioResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new
                        {

                            mensaje = "El usuario indicado no existe en el sistema"

                        });
                    }

                    if (!usuarioResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "Error validando el usuario en la API externa"
                        });
                    }


                    
                    // 3. Convertir JSON a objeto Usuario
                    
                    var usuarioJson = await usuarioResponse.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<Usuario>(usuarioJson);

                    if (usuario == null)
                    {
                        return Results.BadRequest(new { mensaje = "Error leyendo datos del usuario" });
                    }


                    
                    // 4. Validar que su rol sea Estudiante
                    
                    if (!string.Equals(usuario.Rol_Usuario, "Estudiante", StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "El usuario no tiene rol de estudiante"
                        });
                    }


                    // 4. Completar datos del expediente desde la API

                    expediente.tipo_identificacion = usuario.Tipo_Identificacion;
                    expediente.email = usuario.Email;
                    expediente.nombre = usuario.Nombre;

                    expediente.Accion = "Crear";

                    // 5. Crear expediente

                    var result = await service.CRUDExpediente(expediente);

                    // Registrar en bitacora

                    await service.RegistrarBitacoraAsync(
                        
                        accion: "Crear Expediente",
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

            // Actualizar Expediente

            group.MapPut("/", async ([FromServices] Services.IExpediente_EstudianteService service, [FromBody] Entities.Expediente_Estudiantes expediente, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    
                    // 1. Validar Token
                    
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                        


                    
                    // 2. Consultar API de usuarios
                    
                    var usuarioRequest = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5000/usuario/id/{expediente.numero_identificacion}");

                    usuarioRequest.Headers.Add("access_token", accessToken);

                    var usuarioResponse = await httpClient.SendAsync(usuarioRequest);

                    if (usuarioResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "El usuario indicado no existe en el sistema"
                        });
                    }

                    if (!usuarioResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "Error validando el usuario en la API externa"
                        });
                    }


                    
                    // 3. Convertir JSON a Usuario
                    
                    var usuarioJson = await usuarioResponse.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<Usuario>(usuarioJson);

                    if (usuario == null)
                    {
                        return Results.BadRequest(new { mensaje = "Error leyendo datos del usuario" });
                    }


                    
                    // 4. Validar Rol Estudiante
                   
                    if (!string.Equals(usuario.Rol_Usuario, "Estudiante", StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.BadRequest(new
                        {
                            mensaje = "El usuario no tiene rol de estudiante"
                        });
                    }


                    
                    // 5. Completar datos desde API
                    
                    expediente.tipo_identificacion = usuario.Tipo_Identificacion;
                    expediente.email = usuario.Email;
                    expediente.nombre = usuario.Nombre;

                    expediente.Accion = "Actualizar";

                    var (expedienteAntes, m_a) = await service.Obtener_Expediente_Por_ID(expediente.numero_identificacion);

                    var expedienteactualizado = await service.CRUDExpediente(expediente);

                    var (usuarioDespues, m_d) = await service.Obtener_Expediente_Por_ID(expediente.numero_identificacion);

                    var antesYDespues = new
                    {
                        Antes = expedienteAntes,
                        Despues = usuarioDespues
                    };

                    string descripcionJson = JsonSerializer.Serialize(antesYDespues);

                    await service.RegistrarBitacoraAsync(
                        
                        accion: "Actualizar Expediente",
                        descripcion: descripcionJson,
                        accessToken: accessToken
                        );

                    // 6. Actualizar expediente

                    // var result = await service.CRUDExpediente(expediente);

                    return expedienteactualizado;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("UpdateExpediente")
            .WithOpenApi();

            // Eliminar expediente

            group.MapDelete("/{id}", async (string id, [FromServices] Services.IExpediente_EstudianteService service,[FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }

                    var expediente = new Entities.Expediente_Estudiantes
                    {
                        numero_identificacion = id,
                        Accion = "Eliminar"
                    };

                    
                    var result = await service.CRUDExpediente(expediente);

                    // Registrar en bitacora

                    await service.RegistrarBitacoraAsync(
                        
                        accion: "Eliminar Expediente",
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

            #region "CRUD DESACTUALIZADA"

            // Crear Expediente


            //group.MapPost("/", async (
            //    [FromServices] Services.IExpediente_EstudianteService service,
            //    [FromBody] Entities.Expediente_Estudiantes expediente, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            //{
            //    try
            //    {
            //        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
            //        request.Headers.Add("access_token", accessToken);

            //        var response = await httpClient.SendAsync(request);

            //        if (!response.IsSuccessStatusCode)
            //        {

            //            return Results.Unauthorized();

            //        }
            //        expediente.Accion = "Crear";
            //        var result = await service.CRUDExpediente(expediente);
            //        return result;
            //    }
            //    catch (Exception ex)
            //    {
            //        return Results.BadRequest(new { mensaje = ex.Message });
            //    }
            //})
            //.WithName("RealizarExpediente")
            //.WithOpenApi();


            // Actualizar Expediente


            //   group.MapPut("/", async (
            //    [FromServices] Services.IExpediente_EstudianteService service,
            //    [FromBody] Entities.Expediente_Estudiantes expediente, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            //   {
            //       try
            //       {
            //           var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
            //           request.Headers.Add("access_token", accessToken);

            //           var response = await httpClient.SendAsync(request);

            //           if (!response.IsSuccessStatusCode)
            //           {

            //               return Results.Unauthorized();

            //           }
            //           expediente.Accion = "Actualizar";
            //           var result = await service.CRUDExpediente(expediente);
            //           return result;
            //       }
            //       catch (Exception ex)
            //       {
            //           return Results.BadRequest(new { mensaje = ex.Message });
            //       }
            //   })
            //.WithName("UpdateExpediente")
            //.WithOpenApi();




            //// Eliminar expediente
            //group.MapDelete("/", async (
            // [FromServices] Services.IExpediente_EstudianteService service,
            // [FromBody] Entities.Expediente_Estudiantes expediente, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            //{
            //    try
            //    {
            //        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
            //        request.Headers.Add("access_token", accessToken);

            //        var response = await httpClient.SendAsync(request);

            //        if (!response.IsSuccessStatusCode)
            //        {

            //            return Results.Unauthorized();

            //        }
            //        expediente.Accion = "Eliminar";
            //        var result = await service.CRUDExpediente(expediente);
            //        return result;
            //    }
            //    catch (Exception ex)
            //    {
            //        return Results.BadRequest(new { mensaje = ex.Message });
            //    }
            //});

            #endregion

        }

    }
}
