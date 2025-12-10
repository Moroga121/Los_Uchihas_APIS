using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using USR4_Modulos.Entities;
using USR4_Modulos.Services;

namespace USR4_Modulos
{
    public static class ModuloEndpoint
    {

        public static void MapModuloEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/modulo").WithTags(nameof(Modulos));


            #region "Get Parametros"

            #region "Con Validate"

            #region "Obtener todos los modulos con Validate "

            group.MapGet("/", async ([FromServices] IModulosService moduloService, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var modulos = await moduloService.Obtener_Todos_Los_Modulos();
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los modulos",
                   descripcion: modulos,
                   accessToken: accessToken
               );
                return Results.Ok(modulos);
            })
             .WithName("ObtenertodoslosModulos")
             .WithOpenApi();

            #endregion

            #region "Obtener todos los roles con modulos relacionados con Validate "

            group.MapGet("/por-rol-usuario", async ([FromServices] IModulosService moduloService, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var rol_usuarios = await moduloService.Obtener_Todos_Los_Rol_Usuario();
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los roles y modulos asociados",
                   descripcion: rol_usuarios,
                   accessToken: accessToken
               );
                return Results.Ok(rol_usuarios);
            })
             .WithName("ObtenerTodosLosRolUsuario")
             .WithOpenApi();

            #endregion

            #region "Obtener modulo por ID con Validate"

            group.MapGet("/por-id/{id}", async ([FromServices] IModulosService moduloService, string id, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var (modulo, mensaje) = await moduloService.Obtener_Modulo_Por_ID(id);

                if (modulo == null)
                {
                    return Results.NotFound(new { mensaje });
                }
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Obtener modulo por ID",
                   descripcion: modulo,
                   accessToken: accessToken
               );
                return Results.Ok(modulo);
            })
            .WithName("ObtenerModuloPorID")
            .WithOpenApi();

            #endregion

            #region "Obtener modulo por Rol con Validate"

            group.MapGet("/por-rol/{rol}", async ([FromServices] IModulosService moduloService, string rol, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var (modulos, mensaje) = await moduloService.Obtener_Modulo_Por_Rol(rol);

                if (modulos == null)
                {
                    return Results.NotFound(new { mensaje });
                }
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Obtener modulo por rol",
                   descripcion: modulos,
                   accessToken: accessToken
               );
                return Results.Ok(modulos);
            })
            .WithName("ObtenerModuloPorRol")
            .WithOpenApi();

            #endregion

            #endregion

            #endregion

            #region "CRUD"

            #region "Con Validate"

            #region "Post Modulo con Validate"


            group.MapPost("/", async (
             HttpContext context,
             [FromServices] IModulosService moduloService,
             [FromBody] Modulos modulo,
             [FromHeader(Name = "access_token")] string accessToken,
             [FromServices] HttpClient httpClient) =>
                    {
                        // Validación del token
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                        request.Headers.Add("access_token", accessToken);

                        var response = await httpClient.SendAsync(request);
                        if (!response.IsSuccessStatusCode)
                            return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                        // Ejecutar la acción
                        modulo.Accion = "Insert";
                        var modulocreado = await moduloService.CRUD_ModulosAsync(modulo);

                        await moduloService.RegistrarBitacoraAsync(accion: "Crear modulo", descripcion: modulocreado, accessToken: accessToken);

                        return modulocreado;
                    })
         .WithName("InsertModulo")
         .WithOpenApi();

            #endregion

            #region "Put Modulo con Validate"

            group.MapPut("/", async ([FromServices] IModulosService moduloService, [FromBody] Modulos modulo, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                modulo.Accion = "Update";

                var moduloAntes = await moduloService.Obtener_Modulo_Por_ID(modulo.Identificador_Modulo);

                var moduloactualizado = await moduloService.CRUD_ModulosAsync(modulo);

                var moduloDespues = await moduloService.Obtener_Modulo_Por_ID(modulo.Identificador_Modulo);

                var antesYDespues = new
                {
                    Antes = moduloAntes,
                    Despues = moduloDespues
                };

                string descripcionJson = JsonSerializer.Serialize(antesYDespues);
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Actualizar modulo",
                   descripcion: descripcionJson,
                   accessToken: accessToken
               );
                return moduloactualizado;

            })
            .WithName("UpdateModulo")
            .WithOpenApi();

            #endregion

            #region "Delete Modulo con Validate"

            group.MapDelete("/{id}", async ([FromServices] IModulosService moduloService, string id, [FromHeader(Name = "access_token")] string accessToken, [FromServices] HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var modulo = new Modulos
                {
                    Identificador_Modulo = id,
                    Accion = "Delete"
                };
                var moduloeliminado = await moduloService.CRUD_ModulosAsync(modulo);
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Eliminar modulo",
                   descripcion: moduloeliminado,
                   accessToken: accessToken
               );
                return moduloeliminado;

            })
            .WithName("DeleteModulo")
            .WithOpenApi();

            group.MapDelete("/", () =>
            {
                return Results.BadRequest(new { mensaje = "Debe especificar el identificador del módulo en la URL para eliminar." });
            })
           .WithName("DeleteModuloinID")
           .WithOpenApi();

            #endregion

            #endregion

            #region Actualizar permisos de rol con Validate
            group.MapPost("/actualizar-permisos", async (
                [FromBody] Rol_Modulo roles_modulos,
                [FromServices] IModulosService moduloService,
                [FromHeader(Name = "access_token")] string accessToken,
                [FromServices] HttpClient httpClient) =>
            {
                // Validar token llamando al endpoint de validación
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Unauthorized();
                }

                // Llamar al servicio que maneja la lógica
                var resultado = await moduloService.ActualizarPermisosAsync(
                    roles_modulos.Identificador_Modulo,
                    roles_modulos.Roles
                );
                // Registrar intento exitoso en la bitacora del login
                await moduloService.RegistrarBitacoraAsync(
                   accion: "Actualzar permisos modulo",
                   descripcion: resultado,
                   accessToken: accessToken
               );
                return resultado;

            })
            .WithName("ActualizarPermisosModulo")
            .WithOpenApi();
            #endregion

            #endregion

        }


    }
}
