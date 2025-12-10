using ACA1_Promedio.Entities;
using ACA1_Promedio.Services;
using Microsoft.AspNetCore.Mvc;

namespace ACA1_Promedio
{
    public static class HistorialAcademicoEndpoint
    {
        public static void MapHistorialAcademicoEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/historialacademico").WithTags(nameof(HistorialAcademico));

            #region Obtener historial académico por estudiante
            group.MapGet("/{tipoIdentificacion}/{numeroIdentificacion}",
                async ([FromHeader(Name = "access_token")] string accessToken,
                       HttpClient httpClient,
                       [FromServices] HistorialService service,
                       string tipoIdentificacion,
                       string numeroIdentificacion,
                       int? año,
                       string? periodo) =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                    }

                    var historial = await service.ObtenerHistorialAsync(tipoIdentificacion, numeroIdentificacion, año, periodo);

                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener todo el historial",
                       descripcion: historial,
                       accessToken: accessToken
                    );

                    if (!historial.Any())
                        return Results.NotFound(new { Mensaje = "No se encontraron registros académicos para este estudiante." });

                    return Results.Ok(historial);
                })
            .WithName("ObtenerHistorialAcademico")
            .WithOpenApi();
            #endregion
        }

    }
}
