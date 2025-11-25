using Microsoft.AspNetCore.Mvc;
using Módulo_de_Oferta_académica_ACD3.Entities;
using Módulo_de_Oferta_académica_ACD3.Services;

namespace Módulo_de_Oferta_académica_ACD3
{
    public static class CursoEndpoint
    {
        public static void MapCursoEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/curso").WithTags(nameof(Curso));

            group.MapGet("/", async ([FromServices] ICursoService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var lista = await service.Obtener_Todos_Los_Cursos();
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener todos los cursos",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });

            group.MapGet("/{id}", async ([FromServices] ICursoService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var (curso, mensaje) = await service.Obtener_Curso_Por_ID(id);
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener curso por ID",
                   descripcion: curso,
                   accessToken: accessToken
                );
                return curso is null ? Results.NotFound(new { mensaje }) : Results.Ok(curso);
            });

            group.MapGet("/carrera/{idCarrera}", async ([FromServices] ICursoService service, string idCarrera, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                if (string.IsNullOrWhiteSpace(idCarrera))
                    return Results.BadRequest(new { mensaje = "El identificador de la carrera no puede estar vacío." });

                var lista = await service.Obtener_Cursos_Por_Carrera(idCarrera);

                if (lista == null || !lista.Any())
                    return Results.NotFound(new { mensaje = $"No se encontraron cursos para la carrera." });
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener cursos por carrera",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });

            group.MapPost("/", async ([FromServices] ICursoService service, [FromBody] Curso curso, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                curso.Accion = "I";
                var resultado = await service.CRUD_CursosAsync(curso);
                await service.RegistrarBitacoraAsync(
                   accion: "Crear nuevo curso",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });

            group.MapPut("/", async ([FromServices] ICursoService service, [FromBody] Curso curso, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                curso.Accion = "U";
                var resultado = await service.CRUD_CursosAsync(curso);
                await service.RegistrarBitacoraAsync(
                   accion: "Actualizar curso",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });

            group.MapDelete("/{id}", async ([FromServices] ICursoService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var curso = new Curso { ID_Curso = id, Accion = "D" };
                var resultado = await service.CRUD_CursosAsync(curso);
                await service.RegistrarBitacoraAsync(
                   accion: "Eliminar curso",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });


            #region "Validar que exista el Curso"

            group.MapGet("/validar", async ([FromServices] ICursoService service, [FromQuery] string nombre, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                var cursos = await service.Obtener_Todos_Los_Cursos();

                var encontrado = cursos.FirstOrDefault(c => c.Nombre == nombre);

                if (encontrado == null)
                    return Results.NotFound(new { existe = false, mensaje = "El curso no existe." });

                // Devuelve el objeto completo
                return Results.Ok(encontrado);
            });

            //group.MapGet("/validar", async ([FromServices] ICursoService service, [FromQuery] string nombre, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            //{
            //    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
            //    request.Headers.Add("access_token", accessToken);

            //    var response = await httpClient.SendAsync(request);

            //    if (!response.IsSuccessStatusCode)
            //    {

            //        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

            //    }

            //    // Buscar el curso por nombre

            //    var cursos = await service.Obtener_Todos_Los_Cursos();

            //    var encontrado = cursos.FirstOrDefault(c => c.Nombre == nombre);

            //    if (encontrado == null)
            //    {

            //        return Results.NotFound(new { existe = false, mensaje = "El curso no existe." });

            //    }


            //    return Results.Ok(new { existe = true, id = encontrado.ID_Curso });

            //});

            #endregion

        }
    }
}
