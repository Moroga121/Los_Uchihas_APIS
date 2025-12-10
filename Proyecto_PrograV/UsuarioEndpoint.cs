using Microsoft.AspNetCore.Mvc;
using Proyecto_PrograV.Entities;
using Proyecto_PrograV.Services;
using System.Text.Json;

namespace Proyecto_PrograV
{
    public static class UsuarioEndpoint
    {

        public static void MapUsuarioEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/usuario").WithTags(nameof(Usuario));

            #region "Get Usuarios"

            #region "Con Validate"

            #region "Obtener Tipos de Identificación con Método Validate"


            group.MapGet("/tipos_identificacion", async ([FromServices] IUsuarioService usuarioService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                
                var tiposIdentificacion = await usuarioService.Obtener_Tipos_De_Identificacion();
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los tipos de identificacion",
                   descripcion: tiposIdentificacion,
                   accessToken: accessToken
               );
                return Results.Ok(tiposIdentificacion);
            })
            .WithName("ObtenerTiposIdentificacion")
             .WithOpenApi();


            #endregion

            #region "Cambiar Contraseña Usuario con Validate"

            group.MapPut("/cambiar_contrasena", async ([FromServices] IUsuarioService usuarioService, [FromBody] Usuario usuario, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                // Validar token
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }
                // Llamar al servicio para cambiar contraseña

                var resultado = await usuarioService.Cambiar_Contrasena_UsuarioAsync(usuario);
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Cambiar contraseña",
                   descripcion: resultado,
                   accessToken: accessToken
               );

                return resultado;
            })
            .WithName("CambiarContrasenaUsuario")
            .WithOpenApi();

            #endregion

            #region "Obtener Dominios con Método Validate"

            group.MapGet("/dominios", async ([FromServices] IUsuarioService usuarioService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var dominios = await usuarioService.Obtener_Dominios();
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los dominios",
                   descripcion: dominios,
                   accessToken: accessToken
               );

                return Results.Ok(dominios);
            })
            .WithName("ObtenerDominios")
             .WithOpenApi();



            #endregion

            #region "Método Obtener Todos los Usuarios Con Método Validate"

            group.MapGet("/", async ([FromServices] IUsuarioService usuarioService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {


                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }




                var usuarios = await usuarioService.Obtener_Todos_Los_Usuarios();
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener todos los usuarios",
                   descripcion: usuarios,
                   accessToken: accessToken
               );

                return Results.Ok(usuarios);
            })
             .WithName("ObtenerTodosUsuarios")
             .WithOpenApi();

            #endregion

            #region "Obtener Usuario por ID con Método Validate"

            group.MapGet("/id/{id}", async ([FromServices] IUsuarioService usuarioService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                var (usuario, mensaje) = await usuarioService.Obtener_Usuario_Por_Identificacion(id);

                if (usuario == null) return Results.NotFound(new { mensaje });
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener usuario por ID",
                   descripcion: usuario,
                   accessToken: accessToken
               );

                return Results.Ok(usuario);
            })
            .WithName("ObtenerUsuarioPorIdentificacion")
            .WithOpenApi();

            group.MapGet("/{id}", async ([FromServices] IUsuarioService usuarioService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                var (usuario, mensaje) = await usuarioService.Obtener_Usuario_Por_ID(id);

                if (usuario == null) return Results.NotFound(new { mensaje });

                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener usuario por ID",
                   descripcion: usuario,
                   accessToken: accessToken
               );

                return Results.Ok(usuario);
            })
            .WithName("ObtenerUsuarioPorID")
            .WithOpenApi();

            #endregion

            #region "Filtrar Usuario con Método Validate"


            group.MapGet("/filtrar", async ([FromServices] IUsuarioService usuarioService, string identificacion, string nombre, string rol, string tipo, string dominio, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }
                var usuarios = await usuarioService.Obtener_Usuarios_FiltradosAsync(identificacion, nombre, rol, tipo, dominio);
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Obtener usuarios filtrados",
                   descripcion: usuarios,
                   accessToken: accessToken
               );
                return usuarios;
            })
            .WithName("ObtenerUsuariosFiltrados")
            .WithOpenApi();


            #endregion


            #endregion

            #endregion

            #region "CRUD"

            #region "Con Validate"

            #region "Post Usuarios con Validate"

            group.MapPost("/", async ([FromServices] IUsuarioService usuarioService, [FromBody] Usuario usuario, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                usuario.Accion = "Insert";
                var usuariocreado = await usuarioService.CRUD_UsuariosAsync(usuario);
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Crear Usuario",
                   descripcion: usuariocreado,
                   accessToken: accessToken
               );
                return usuariocreado;

            })
            .WithName("InsertUsuario")
            .WithOpenApi();

            #endregion

            #region "Put Usuarios con Validate"

            group.MapPut("/", async ([FromServices] IUsuarioService usuarioService, [FromBody] Usuario usuario, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                usuario.Accion = "Update";

                var (usuarioAntes, m_a) = await usuarioService.Obtener_Usuario_Por_ID(usuario.Email);

                var usuarioactualizado = await usuarioService.CRUD_UsuariosAsync(usuario);

                var (usuarioDespues, m_d) = await usuarioService.Obtener_Usuario_Por_ID(usuario.Email);

                var antesYDespues = new
                {
                    Antes = usuarioAntes,
                    Despues = usuarioDespues
                };

                string descripcionJson = JsonSerializer.Serialize(antesYDespues);

                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Actualizar Usuario",
                   descripcion: descripcionJson,
                   accessToken: accessToken
               );
                return usuarioactualizado;
            })
            .WithName("UpdateUsuario")
            .WithOpenApi();

            #endregion

            #region "Delete Usuarios con Validate"


            group.MapDelete("/{id}", async ([FromServices] IUsuarioService usuarioService, string id, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }

                var usuario = new Usuario { Identificacion = id, Accion = "Delete" };

                var usuarioeliminado = await usuarioService.CRUD_UsuariosAsync(usuario);
                // Registrar intento exitoso en la bitacora del login
                await usuarioService.RegistrarBitacoraAsync(
                   accion: "Eliminar Usuario",
                   descripcion: usuarioeliminado,
                   accessToken: accessToken
               );
                return usuarioeliminado;
            })
            .WithName("DeleteUsuario")
            .WithOpenApi();


            group.MapDelete("/", () =>
            {
                return Results.BadRequest(new { mensaje = "Debe especificar el identificador del usuario en la URL para eliminar." });
            })
            .WithName("DeleteUsuariosinID")
            .WithOpenApi();


            #endregion


            #endregion

            #endregion

        }


    }
}
