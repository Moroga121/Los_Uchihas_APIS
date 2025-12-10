using GEN01_Bitacora.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace GEN01_Bitacora
{
    public static class BitacoraEndPoint
    {
        public static void MapBitacoraEndpoints(this IEndpointRouteBuilder routes)
        {
            var grupo = routes.MapGroup("/bitacora").WithTags("Bitacora");

            grupo.MapPost("/registrar", async (
                [FromBody] JsonElement json,
                [FromServices] Services.IBitacoraService bitacoraService,
                [FromHeader(Name = "access_token")] string? accessToken,
                HttpClient httpClient) =>
            {
                string usuario = "";
                bool tokenValido = false;

                // Si viene un token → intentar validarlo
                if (!string.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                        request.Headers.Add("access_token", accessToken);

                        var response = await httpClient.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            // Token válido hay que obtener el usario desde el token
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(accessToken);
                            usuario = jwtToken.Claims.FirstOrDefault(c =>
                                c.Type == "email" || c.Type == "sub" || c.Type == "Usuario")?.Value ?? "";
                            tokenValido = true;
                        }
                        else
                        {
                            return Results.Unauthorized();
                        }
                    }
                    catch
                    {
                        return Results.Unauthorized();
                    }
                }

                if (!tokenValido)
                {
                    usuario = json.TryGetProperty("Usuario", out var usuarioJson)
                        ? usuarioJson.GetString() ?? ""
                        : "Desconocido";
                }

                // Obtener los demás campos
                string accion = json.GetProperty("Accion").GetString() ?? "";
                object descripcion = json.GetProperty("Descripcion").ValueKind switch
                {
                    JsonValueKind.Object => json.GetProperty("Descripcion"),
                    JsonValueKind.String => json.GetProperty("Descripcion").GetString(),
                    _ => json.GetProperty("Descripcion").ToString()
                };

                // Registrar en bitácora
                await bitacoraService.Registrar_Bitacora(usuario, accion, descripcion);

                var nuevaBitacora = new Entities.Bitacora
                {
                    Usuario = usuario,
                    Accion = accion,
                    Descripcion = descripcion?.ToString(),
                    Fecha = DateTime.UtcNow
                };

                return Results.Created($"/bitacora/{Guid.NewGuid()}", nuevaBitacora);
            })
            .WithName("Registrar_Bitacora")
            .WithOpenApi();


           


            grupo.MapGet("/obtener_todas", async ([FromServices] IBitacoraService bitacoraService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);


                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                    }

                    
                    var bitacoras = await bitacoraService.Obtener_Todas_Las_Bitacoras();

                    if (bitacoras == null || !bitacoras.Any())
                    {
                        return Results.NoContent();
                    }

                    return Results.Ok(bitacoras);
                }
                catch (Exception ex)
                {
                    return Results.Json(new
                    {
                        mensaje = "Error al obtener las bitácoras",
                        detalle = ex.Message
                    }, statusCode: 500);
                }
            })
            .WithName("Obtener_Bitacoras")
            .WithOpenApi();

            grupo.MapGet("/obtener_todas-filtradas", async (
            [FromServices] IBitacoraService bitacoraService,
            [FromHeader(Name = "access_token")] string accessToken,
            [FromQuery] DateOnly? fechaInicio,
            [FromQuery] DateOnly? fechaFin,
            [FromQuery] string? accion,
            [FromQuery] string? usuario,
            HttpClient httpClient) =>
                    {
                        try
                        {
                            // Validar el token con el microservicio de login
                            var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                            request.Headers.Add("access_token", accessToken);

                            var response = await httpClient.SendAsync(request);

                            if (!response.IsSuccessStatusCode)
                            {
                                return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                            }

                            // Obtener las bitácoras según los filtros
                            var bitacoras = await bitacoraService.Obtener_Todas_Las_BitacorasFiltradas(
                                fechaInicio,
                                fechaFin,
                                usuario,
                                accion
                            );

                            if (bitacoras == null || !bitacoras.Any())
                                return Results.NoContent();

                            return Results.Ok(bitacoras);
                        }
                        catch (Exception ex)
                        {
                            return Results.Json(new
                            {
                                mensaje = "Error al obtener las bitácoras",
                                detalle = ex.Message
                            }, statusCode: 500);
                        }
                    })
        .WithName("Obtener_Bitacoras_Filtradas")
        .WithOpenApi();



        }
    }
}
