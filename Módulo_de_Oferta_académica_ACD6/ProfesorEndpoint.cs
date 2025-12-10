using Microsoft.AspNetCore.Mvc;
using Módulo_de_Oferta_académica_ACD6.Entities;
using Módulo_de_Oferta_académica_ACD6.Services;

namespace Módulo_de_Oferta_académica_ACD6
{
    public static class ProfesorEndpoint
    {
        public static void MapProfesorEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/profesor").WithTags(nameof(Profesor));

            group.MapGet("/", async ([FromServices] IProfesorService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var lista = await service.Obtener_Todos();
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener todos los profesores",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });

            group.MapGet("/{id}", async ([FromServices] IProfesorService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var (profesor, mensaje) = await service.Obtener_Por_ID(id);
                if (profesor == null)
                    return Results.NotFound(new { mensaje });
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener profesor por ID",
                   descripcion: profesor,
                   accessToken: accessToken
                );
                return Results.Ok(profesor);
            });

            group.MapPost("/", async ([FromServices] IProfesorService service, [FromBody] Profesor profesor, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                profesor.Accion = "I";
                var resultado = await service.CRUD_ProfesoresAsync(profesor);
                await service.RegistrarBitacoraAsync(
                   accion: "Crear nuevo profesor",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });

            group.MapPut("/", async ([FromServices] IProfesorService service, [FromBody] Profesor profesor, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                profesor.Accion = "U";
                var resultado = await service.CRUD_ProfesoresAsync(profesor);
                await service.RegistrarBitacoraAsync(
                   accion: "Actualizar profesor",
                   descripcion: resultado,
                   accessToken: accessToken
                );

                return (resultado);
            });

            group.MapDelete("/{id}", async (
                [FromServices] IProfesorService service,
                string id,
                [FromHeader(Name = "access_token")] string accessToken,
                HttpClient httpClient
            ) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                    var profesor = new Profesor { ID_Profesor = id, Accion = "D" };
                    var resultado = await service.CRUD_ProfesoresAsync(profesor);
                    await service.RegistrarBitacoraAsync(
                       accion: "Eliminar profesor",
                       descripcion: resultado,
                       accessToken: accessToken
                    );
                    if (resultado is IResult r)
                        return r;

                    return Results.Json(new
                    {
                        mensaje = "Operación completada correctamente (sin contenido adicional del servidor)."
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar profesor: {ex.Message}");
                    return Results.Json(
                        new { mensaje = $"Error interno del servidor: {ex.Message}" },
                        statusCode: 500
                    );
                }
            });


            group.MapGet("/buscar", async (
                [FromServices] IProfesorService service,
                [FromHeader(Name = "access_token")] string accessToken,
                HttpClient httpClient,
                [FromQuery] string busqueda = "",
                [FromQuery] string ordenCampo = "Nombre",
                [FromQuery] string ordenDireccion = "ASC",
                [FromQuery] int pagina = 1,
                [FromQuery] int tamanoPagina = 10
            ) =>
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                req.Headers.Add("access_token", accessToken);
                var res = await httpClient.SendAsync(req);
                if (!res.IsSuccessStatusCode)
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                var lista = await service.BuscarProfesoresAsync(busqueda, ordenCampo, ordenDireccion, pagina, tamanoPagina);
                await service.RegistrarBitacoraAsync(
                   accion: "Buscar profesores",
                   descripcion: new
                   {
                       busqueda,
                       ordenCampo,
                       ordenDireccion,
                       pagina,
                       tamanoPagina,
                   },
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });
        }
    }
}
