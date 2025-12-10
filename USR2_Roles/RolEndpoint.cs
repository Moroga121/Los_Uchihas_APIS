using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
using USR2_Roles.Entities;
using USR2_Roles.Services;


namespace USR2_Roles
{
    public static class RolEndpoint
    {

        public static void MapRolEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/rol").WithTags(nameof(Rol));

            #region "Get Usuarios"

            #region "Con Validate"

            #region "Obtener todos los roles Con Validate"


            group.MapGet("/", async ([FromServices] IRolService usuarioService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                var roles = await usuarioService.Obtener_Todos_Los_Roles();
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los roles",
                   descripcion: roles,
                   accessToken: accessToken
               );
                return Results.Ok(roles);
            })
            .WithName("ObtenertodoslosRoles")
            .WithOpenApi();




            #endregion

            #region "Obtener roles por ID Con Validate"

            group.MapGet("/{id}", async ([FromServices] IRolService usuarioService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var (rol, mensaje) = await usuarioService.Obtener_Rol_Por_ID(id);

                if (rol == null)
                {

                    return Results.NotFound(new { mensaje });

                }
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener rol por id",
                   descripcion: rol,
                   accessToken: accessToken
               );

                return Results.Ok(rol);
            })
            .WithName("ObtenerRolPorID")
            .WithOpenApi();


            #endregion

            #endregion


            #endregion

            #region "CRUD"

            #region "Con Validate"

            #region "Post Usuarios con Validate"


            group.MapPost("/", async ([FromServices] IRolService usuarioService, [FromBody] Rol rol, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                rol.Accion = "Insert";
                var rolcreado = await usuarioService.CRUD_RolesAsync(rol);
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Crear rol",
                   descripcion: rolcreado,
                   accessToken: accessToken
               );
                return rolcreado;
            })
             .WithName("InsertRol")
             .WithOpenApi();

            #endregion

            #region "Put Usuarios con Validate"


            group.MapPut("/", async ([FromServices] IRolService usuarioService, [FromBody] Rol rol, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                rol.Accion = "Update";




                var (rolAntes, m_a) = await usuarioService.Obtener_Rol_Por_ID(rol.Identificador_Rol);

                var rolactualizado = await usuarioService.CRUD_RolesAsync(rol);

                var (rolDespues, m_d) = await usuarioService.Obtener_Rol_Por_ID(rol.Identificador_Rol);

                var antesYDespues = new
                {
                    Antes = rolAntes,
                    Despues = rolDespues
                };

                string descripcionJson = JsonSerializer.Serialize(antesYDespues);

                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Actualizar rol",
                   descripcion: descripcionJson,
                   accessToken: accessToken
               );
                return rolactualizado;
            })
            .WithName("UpdateRol")
            .WithOpenApi();

            #endregion

            #region "Delete Usuarios con Validate"

            group.MapDelete("/{id}", async ([FromServices] IRolService usuarioService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }


                var rol = new Rol { Identificador_Rol = id, Accion = "Delete" };

                var roleliminado = await usuarioService.CRUD_RolesAsync(rol);
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Eliminar rol",
                   descripcion: roleliminado,
                   accessToken: accessToken
               );

                return roleliminado;
            })
            .WithName("DeleteRol")
            .WithOpenApi();

            group.MapDelete("/", () =>
            {
                return Results.BadRequest(new { mensaje = "Debe especificar el identificador del rol en la URL para eliminar." });
            })
            .WithName("DeleteRolSinId")
            .WithOpenApi();

            #endregion

            #endregion

            #region Actualizar permisos de rol con Validate
            group.MapPost("/actualizar-permisos", async (
                [FromBody] Rol_Modulo roles_modulos,
                [FromServices] IRolService rolService,
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
                var resultado = await rolService.ActualizarPermisosAsync(
                    roles_modulos.Identificador_Rol,
                    roles_modulos.Modulos
                );
                // Registrar intento exitoso en la bitacora del login
                await rolService.RegistrarBitacoraAsync(
                   accion: "Asignar permisos rol",
                   descripcion: resultado,
                   accessToken: accessToken
               );
                return resultado;

            })
            .WithName("ActualizarPermisosRol")
            .WithOpenApi();
            #endregion

            #endregion

        }

    }
}
