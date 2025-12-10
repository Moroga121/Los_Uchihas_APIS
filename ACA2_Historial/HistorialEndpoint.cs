using ACA2_Historial.Entities;
using ACA2_Historial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACA2_Historial
{
    public static class HistorialEndpoint
    {
        public static void MapEstudiantesMatriculaPeriodoEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/listadoestudiantes").WithTags(nameof(Estudiante_Matriculado));




            group.MapGet("/", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IHistorialService historialService, [FromQuery] string periodo) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                var resultado = await historialService.ObtenerEstudiantesMatriculaPeriodoAsync(periodo);

                await historialService.RegistrarBitacoraAsync(
                   accion: "Obtener estudiantes periodo",
                   descripcion: resultado.Estudiante_Matriculado,
                   accessToken: accessToken
                );

                if (!resultado.Exito)
                    return Results.BadRequest(new { mensaje = resultado.Mensaje });

                return Results.Ok(resultado.Estudiante_Matriculado);
            })
           .WithName("ObtenerEstudiantesMatriculaPeriodo")
           .WithOpenApi();


            #region Obtener todas las matrículas
            var groupMatriculas = routes.MapGroup("/matriculas").WithTags("Matriculas");

            groupMatriculas.MapGet("/",
                async ([FromHeader(Name = "access_token")] string accessToken,
                       HttpClient httpClient,
                       [FromServices] IHistorialService service) =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                    }

                    var matriculas = await service.ObtenerMatriculasAsync();

                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener estudiantes periodo",
                       descripcion: matriculas,
                       accessToken: accessToken
                    );



                    if (!matriculas.Any())
                        return Results.NotFound(new { Mensaje = "No se encontraron matrículas registradas." });

                    return Results.Ok(matriculas);
                })
            .WithName("ObtenerMatriculas")
            .WithOpenApi();
            #endregion
        }
    }
}
