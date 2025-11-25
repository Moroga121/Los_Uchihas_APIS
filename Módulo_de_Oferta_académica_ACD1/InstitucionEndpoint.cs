using Módulo_de_Oferta_académica_ACD1.Entities;
using Módulo_de_Oferta_académica_ACD1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Módulo_de_Oferta_académica_ACD1
{
    public static class InstitucionEndpoint
    {
        public static void MapInstitucionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/institucion").WithTags(nameof(Institucion));

            group.MapGet("/", async ([FromServices] IInstitucionService institucionService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var lista = await institucionService.Obtener_Todas_Las_Instituciones();
                await institucionService.RegistrarBitacoraAsync(
                   accion: "Obtener todas las instituciones",
                   descripcion: lista,
                   accessToken: accessToken
               );
                return Results.Ok(lista);
            })
            .WithName("ObtenerTodasLasInstituciones")
            .WithOpenApi();

            group.MapGet("/{id}", async ([FromServices] IInstitucionService institucionService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var (institucion, mensaje) = await institucionService.Obtener_Institucion_Por_ID(id);

                if (institucion == null)
                {
                    return Results.NotFound(new { mensaje });
                }

                await institucionService.RegistrarBitacoraAsync(
                   accion: "Obtener institución por ID",
                   descripcion: institucion,
                   accessToken: accessToken
               );

                return Results.Ok(institucion);
            })
            .WithName("ObtenerInstitucionPorID")
            .WithOpenApi();

            group.MapPost("/", async ([FromServices] IInstitucionService institucionService, [FromBody] Institucion institucion, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                institucion.Accion = "I";
                var resultado = await institucionService.CRUD_InstitucionesAsync(institucion);
                await institucionService.RegistrarBitacoraAsync(
                   accion: "Insertar nueva institución",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);

            })
            .WithName("InsertarInstitucion")
            .WithOpenApi();

            group.MapPut("/", async ([FromServices] IInstitucionService institucionService, [FromBody] Institucion institucion, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                institucion.Accion = "U"; 
                var resultado = await institucionService.CRUD_InstitucionesAsync(institucion);
                await institucionService.RegistrarBitacoraAsync(
                   accion: "Actualizar institución",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado) ;
            })
            .WithName("ActualizarInstitucion")
            .WithOpenApi();

            group.MapDelete("/{id}", async ([FromServices] IInstitucionService institucionService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var institucion = new Institucion
                {
                    ID_Institucion = id,
                    Accion = "D"
                };
                var resultado = await institucionService.CRUD_InstitucionesAsync(institucion);
                await institucionService.RegistrarBitacoraAsync(
                   accion: "Eliminar institución",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            })
            .WithName("EliminarInstitucion")
            .WithOpenApi();

            group.MapGet("/buscar", async ([FromServices] IInstitucionService institucionService,[FromQuery] string? nombre,[FromHeader(Name = "access_token")] string accessToken,HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultados = await institucionService.Buscar_Instituciones_Por_Nombre(nombre ?? "");
                await institucionService.RegistrarBitacoraAsync(
                   accion: "Buscar instituciones por nombre",
                   descripcion: new { nombre },
                   accessToken: accessToken
                );
                return Results.Ok(resultados);
            })
            .WithName("BuscarInstitucionesPorNombre")
            .WithOpenApi();

        }
    }
}
