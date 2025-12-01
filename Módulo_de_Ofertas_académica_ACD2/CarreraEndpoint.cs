using Módulo_de_Ofertas_académica_ACD2.Entities;
using Módulo_de_Ofertas_académica_ACD2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Módulo_de_Ofertas_académica_ACD2
{
    public static class CarreraEndpoint
    {
        public static void MapCarreraEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/carrera").WithTags(nameof(Carrera));

            group.MapGet("/", async ([FromServices] ICarreraService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var lista = await service.Obtener_Todas_Las_Carreras();
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener todas las carreras",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });

            group.MapGet("/{id}", async ([FromServices] ICarreraService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var (carrera, mensaje) = await service.Obtener_Carrera_Por_ID(id);
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener carrera por ID",
                   descripcion: carrera,
                   accessToken: accessToken
                );
                return carrera is null ? Results.NotFound(new { mensaje }) : Results.Ok(carrera);
            });

            group.MapGet("/institucion/{idInstitucion}", async ([FromServices] ICarreraService service, string idInstitucion, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                if (string.IsNullOrWhiteSpace(idInstitucion))
                    return Results.BadRequest(new { mensaje = "El identificador de la institución no puede estar vacío." });

                var lista = await service.Obtener_Carreras_Por_Institucion(idInstitucion);

                if (lista == null || !lista.Any())
                    return Results.NotFound(new { mensaje = $"No se encontraron carreras para la institución." });
                await service.RegistrarBitacoraAsync(
                   accion: "Obtener carreras por ID de institución",
                   descripcion: lista,
                   accessToken: accessToken
                );
                return Results.Ok(lista);
            });

            group.MapPost("/", async ([FromServices] ICarreraService service, [FromBody] Carrera carrera, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                carrera.Accion = "I";
                var resultado = await service.CRUD_CarrerasAsync(carrera);
                await service.RegistrarBitacoraAsync(
                   accion: "Crear nueva carrera",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);

            });

            group.MapPut("/", async ([FromServices] ICarreraService service, [FromBody] Carrera carrera, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                carrera.Accion = "U";
                var resultado = await service.CRUD_CarrerasAsync(carrera);
                await service.RegistrarBitacoraAsync(
                   accion: "Actualizar carrera",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });

            group.MapDelete("/{id}", async ([FromServices] ICarreraService service, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var carrera = new Carrera { ID_Carrera = id, Accion = "D" };
                var resultado = await service.CRUD_CarrerasAsync(carrera);
                await service.RegistrarBitacoraAsync(
                   accion: "Eliminar carrera",
                   descripcion: resultado,
                   accessToken: accessToken
                );
                return (resultado);
            });


            #region "Validar Que exista la Carrera"


            group.MapGet("/validar", async ([FromServices] ICarreraService service, [FromQuery] string nombre, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                // Validar token
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                // Buscar la carrera por nombre
                var carreras = await service.Obtener_Todas_Las_Carreras();
                var encontrada = carreras.FirstOrDefault(c => c.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

                if (encontrada == null)
                {
                    return Results.NotFound(new { mensaje = "La carrera no existe" });
                }

                // Devuelve el objeto completo
                return Results.Ok(encontrada);
            });

            //group.MapGet("/validar", async ([FromServices] ICarreraService service, [FromQuery] string nombre, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            //{
            //    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
            //    request.Headers.Add("access_token", accessToken);

            //    var response = await httpClient.SendAsync(request);

            //    if (!response.IsSuccessStatusCode)
            //    {

            //        return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

            //    }


            //    var carreras = await service.Obtener_Todas_Las_Carreras();
            //    var encontrada = carreras.FirstOrDefault(c => c.Nombre == nombre);

            //    if (encontrada == null)
            //    {

            //        return Results.NotFound(new { existe = false });


            //    }

            //    return Results.Ok(new { existe = true, id = encontrada.ID_Carrera });
            //});



            #endregion

        }
    }
}
