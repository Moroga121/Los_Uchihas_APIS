using Microsoft.AspNetCore.Mvc;
using Notificaciones.Entities;
using Notificaciones.Services;

namespace Notificaciones
{
    public static class NotificacionEndpoint
    {
        public static void MapNotificacionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/notificar").WithTags(nameof(Notificacion));

            // POST: /notificar
            group.MapPost("/", async (
                [FromHeader(Name = "access_token")] string accessToken,
                HttpClient httpClient,
                INotificacionService service,
                Notificacion notificacion) =>
            {
                try
                {
                    // Validar token de acceso
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                    }

                    // Enviar notificación y obtener resultado
                    var resultado = await service.CrearYEnviarNotificacionAsync(notificacion);

                    // Retornar respuesta según el resultado
                    if (resultado.Success)
                    {
                        return Results.Created($"/notificar", new
                        {
                            exito = true,
                            mensaje = resultado.Message,
                            datos = notificacion
                        });
                    }

                    else
                    {
                        return Results.BadRequest(new
                        {
                            exito = false,
                            mensaje = resultado.Message
                        });
                    }
                }
                catch (ArgumentException ex)
                {
                    // Errores de validación
                    return Results.BadRequest(new
                    {
                        exito = false,
                        mensaje = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    // Errores inesperados
                    return Results.Json(new
                    {
                        exito = false,
                        mensaje = "Error interno del servidor",
                        detalle = ex.Message
                    }, statusCode: 500);
                }
            });

            group.MapGet("/", async (
                [FromHeader(Name = "access_token")] string accessToken,
                HttpClient httpClient,
                INotificacionService service) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                    }

                    var notificaciones = await service.ObtenerNotificacionesAsync();

                    if (!notificaciones.Any())
                    {
                        return Results.NoContent(); // 204
                    }

                    return Results.Ok(notificaciones);
                }
                catch (Exception ex)
                {
                    return Results.Json(new
                    {
                        mensaje = "Error al obtener las notificaciones",
                        detalle = ex.Message
                    }, statusCode: 500);
                }
            });

            // Notificaciones por email

            group.MapGet("{email}", async ([FromHeader(Name = "access_token")] string accessToken, [FromRoute] string email, HttpClient httpClient, INotificacionService service) =>
            {
                try
                {
                    // Validar token
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                    }

                    // Obtener notificaciones por email
                    var notificaciones = await service.ObtenerNotificacionesPorEmailAsync(email);

                    if (!notificaciones.Any())
                    {
                        return Results.NoContent(); // 204
                    }

                    return Results.Ok(notificaciones);
                }
                catch (Exception ex)
                {
                    return Results.Json(new
                    {
                        mensaje = "Error al obtener las notificaciones",
                        detalle = ex.Message
                    }, statusCode: 500);
                }
            });


        }
    }
}
