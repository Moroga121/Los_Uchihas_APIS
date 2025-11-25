using Microsoft.AspNetCore.Mvc;
using Módulo_de_Oferta_académica_ACD4.Entities;
using Módulo_de_Oferta_académica_ACD4.Sercives;

namespace Módulo_de_Oferta_académica_ACD4
{
    public static class GrupoEndpoint
    {
        public static void MapGrupoEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/grupo").WithTags(nameof(Grupo));

            group.MapGet("/", async ([FromServices] IGrupoService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var lista = await service.Obtener_Todos_Los_Grupos();
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener todos los grupos",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });

            group.MapGet("/{id}", async ([FromServices] IGrupoService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var (grupo, mensaje) = await service.Obtener_Grupo_Por_ID(id);
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener grupo por ID",
                   descripcion: grupo,
                   accessToken: accessToken
                );
                return grupo is null ? Results.NotFound(new { mensaje }) : Results.Ok(grupo);
            });

            group.MapPost("/", async ([FromServices] IGrupoService service, [FromBody] Grupo grupo, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                grupo.Accion = "I";
                var resultado = await service.CRUD_GruposAsync(grupo);
                await service.RegistrarBitacoraAsync(
                   accion: "Crear nuevo grupo",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });

            group.MapPut("/", async ([FromServices] IGrupoService service, [FromBody] Grupo grupo, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                grupo.Accion = "U";
                var resultado = await service.CRUD_GruposAsync(grupo);
                await service.RegistrarBitacoraAsync(
                   accion: "Actualizar grupo",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });

            group.MapDelete("/{id}", async ([FromServices] IGrupoService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var grupo = new Grupo { ID_Grupo = id, Accion = "D" };
                var resultado = await service.CRUD_GruposAsync(grupo);
                await service.RegistrarBitacoraAsync(
                   accion: "Eliminar grupo",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });
        }
    }
}
