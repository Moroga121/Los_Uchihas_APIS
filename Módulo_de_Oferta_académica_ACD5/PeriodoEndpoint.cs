using Microsoft.AspNetCore.Mvc;
using Módulo_de_Oferta_académica_ACD5.Entities;
using Módulo_de_Oferta_académica_ACD5.Service;

namespace Módulo_de_Oferta_académica_ACD5
{
    public static class PeriodoEndpoint
    {
        public static void MapPeriodoEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/periodo").WithTags(nameof(Periodo));

            group.MapGet("/", async ([FromServices] IPeriodoService periodoService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var lista = await periodoService.Obtener_Todos_Los_Periodos();
                await periodoService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los periodos",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            })
            .WithName("ObtenerTodosLosPeriodos")
            .WithOpenApi();

            group.MapGet("/{id}", async ([FromServices] IPeriodoService periodoService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var (periodo, mensaje) = await periodoService.Obtener_Periodo_Por_ID(id);

                if (periodo == null)
                    return Results.NotFound(new { mensaje });

                await periodoService.RegistrarBitacoraAsync(
                   accion: "Obtener periodo por ID",
                   descripcion: periodo,
                   accessToken: accessToken
                );
                return Results.Ok(periodo); 
            })
            .WithName("ObtenerPeriodoPorID")
            .WithOpenApi();

            group.MapPost("/", async ([FromServices] IPeriodoService periodoService, [FromBody] Periodo periodo, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                periodo.Accion = "I";
                var resultado = await periodoService.CRUD_PeriodosAsync(periodo);
                await periodoService.RegistrarBitacoraAsync(
                   accion: "Insertar nuevo periodo",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            })
            .WithName("InsertarPeriodo")
            .WithOpenApi();

            group.MapPut("/", async ([FromServices] IPeriodoService periodoService, [FromBody] Periodo periodo, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                periodo.Accion = "U";
                var resultado = await periodoService.CRUD_PeriodosAsync(periodo);
                await periodoService.RegistrarBitacoraAsync(
                   accion: "Actualizar periodo",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            })
            .WithName("ActualizarPeriodo")
            .WithOpenApi();

            group.MapDelete("/{id}", async ([FromServices] IPeriodoService periodoService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var periodo = new Periodo
                {
                    ID_Periodo = id,
                    Accion = "D"
                };
                var resultado = await periodoService.CRUD_PeriodosAsync(periodo);
                await periodoService.RegistrarBitacoraAsync(
                   accion: "Eliminar periodo",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            })
            .WithName("EliminarPeriodo")
            .WithOpenApi();


            #region "Validar Periodo"

            group.MapGet("/validar", async ([FromServices] IPeriodoService service, [FromQuery] string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }


                var (periodo, mensaje) = await service.Obtener_Periodo_Por_ID(id);

                if (periodo == null)
                {
                    return Results.NotFound(new { existe = false });
                }

                // Devuelve la info completa del periodo
                return Results.Ok(periodo);

            });


            #endregion

        }
    }
}
