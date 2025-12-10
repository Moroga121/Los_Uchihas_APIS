using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using USR3_Parametrización.Entities;
using USR3_Parametrización.Services;

namespace USR3_Parametrización
{
    public static class ParametrizacionEndpoint
    {

        public static void MapParametrizacionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/parametro").WithTags(nameof(Parametrizacion));


            #region "Get Parametros"

            #region "Con Validate"


            #region "Obtener todos los parametros con Validate"

            group.MapGet("/", async ([FromServices] IParametrizacionService parametroService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                
                var parametros = await parametroService.Obtener_Todos_Los_Parametros();
                // Registrar intento exitoso en la bitacora del login
                await parametroService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los parametros",
                   descripcion: parametros,
                   accessToken: accessToken
               );

                return Results.Ok(parametros);
            })
            .WithName("ObtenertodoslosParametros")
            .WithOpenApi();

            #endregion

            #region "Obtener parametro por ID con Validate"

            group.MapGet("/{id}", async ([FromServices] IParametrizacionService parametroService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                
                var (parametro, mensaje) = await parametroService.Obtener_Parametro_Por_ID(id);

                if (parametro == null)
                {

                    return Results.NotFound(new { mensaje });

                }

                // Registrar intento exitoso en la bitacora del login
                await parametroService.RegistrarBitacoraAsync(
                   accion: "Obtener parametros por id",
                   descripcion: parametro,
                   accessToken: accessToken
               );
                return Results.Ok(parametro);
            })
            .WithName("ObtenerParametroPorID")
            .WithOpenApi();

            #endregion


            #endregion


            #endregion

            #region "CRUD"


            #region "Con Validate"

            #region "Post Parametrización con Validate"


            group.MapPost("/", async ([FromServices] IParametrizacionService parametrizacionService, [FromBody] Parametrizacion parametrizacion, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                parametrizacion.Accion = "Insert";

                var parametrocreado = await parametrizacionService.CRUD_ParametrizacionAsync(parametrizacion);
                // Registrar intento exitoso en la bitacora del login
                await parametrizacionService.RegistrarBitacoraAsync(
                   accion: "Crear parametro",
                   descripcion: parametrocreado,
                   accessToken: accessToken
               );

                return parametrocreado;
            })
            .WithName("InsertParametro")
            .WithOpenApi();

            #endregion

            #region "Put Parametrización con Validate"


            group.MapPut("/", async ([FromServices] IParametrizacionService parametrizacionService, [FromBody] Parametrizacion parametrizacion, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                parametrizacion.Accion = "Update";

                var (parametroAntes, m_a) = await parametrizacionService.Obtener_Parametro_Por_ID(parametrizacion.Identificador_Parametro);

                var parametroactualizado = await parametrizacionService.CRUD_ParametrizacionAsync(parametrizacion);

                var (parametroDespues, m_d) = await parametrizacionService.Obtener_Parametro_Por_ID(parametrizacion.Identificador_Parametro);


                var antesYDespues = new
                {
                    Antes = parametroAntes,
                    Despues = parametroDespues
                };

                string descripcionJson = JsonSerializer.Serialize(antesYDespues);
                // Registrar intento exitoso en la bitacora del login
                await parametrizacionService.RegistrarBitacoraAsync(
                   accion: "Actualizar parametro",
                   descripcion: descripcionJson,
                   accessToken: accessToken
               );

                return parametroactualizado;
            })
            .WithName("UpdateParametro")
            .WithOpenApi();

            #endregion

            #region "Delete Parametrización con Validate"

            group.MapDelete("/{id}", async ([FromServices] IParametrizacionService parametrizacionService, string id, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var parametrizacion = new Parametrizacion { Identificador_Parametro = id, Accion = "Delete" };

                var parametroeliminado = await parametrizacionService.CRUD_ParametrizacionAsync(parametrizacion);
                // Registrar intento exitoso en la bitacora del login
                await parametrizacionService.RegistrarBitacoraAsync(
                   accion: "Crear parametro",
                   descripcion: parametroeliminado,
                   accessToken: accessToken
               );

                return parametroeliminado;
            })
            .WithName("DeleteParametro")
            .WithOpenApi();

            group.MapDelete("/", () =>
            {
                return Results.BadRequest(new { mensaje = "Debe especificar el identificador del parámetro en la URL para eliminar." });
            })
            .WithName("DeleteParametrosinID")
            .WithOpenApi();

            #endregion


            #endregion



            #endregion

        }

    }
}
