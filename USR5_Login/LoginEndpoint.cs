using Microsoft.AspNetCore.Mvc;
using USR5_Login.Entities;
using USR5_Login.Repository;
using USR5_Login.Services;

namespace USR5_Login
{
    public static class LoginEndpoint
    {

        public static void MapLoginEndpoints(this IEndpointRouteBuilder routes)
        {

            var group = routes.MapGroup("/login").WithTags(nameof(Login));

            #region "Login Usuario"

            group.MapPost("/", async ([FromServices] ILoginService loginService,[FromHeader(Name = "email")] string email, [FromHeader(Name = "contrasena")] string contrasena) =>
            {
                
                var login = new Login
                {
                    Email = email,
                    Contrasena = contrasena
                };

                var loginResult = await loginService.ValidarUsuarioAsync(login);
                return loginResult;
            })
            .WithName("LoginUsuario")
            .WithOpenApi();

            #endregion

            #region "Refresh Token"

            group.MapPost("/refresh", async ([FromServices] TokenRepository tokenRepo, [FromHeader(Name = "refresh_token")] string refreshToken) =>
            {
                var nuevoToken = tokenRepo.RefreshToken(refreshToken);

                if (nuevoToken == null)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }
                    

                return Results.Created("/login/refresh", new
                {
                    expires_in = nuevoToken.Expires_In,
                    access_token = nuevoToken.Access_Token,
                    refresh_token = nuevoToken.Refresh_Token
                });
            })
            .WithName("RefreshToken")
            .WithOpenApi();


            #endregion

            #region "Validate Token"

            group.MapPost("/validate", ([FromServices] TokenRepository tokenRepo, [FromHeader(Name = "access_token")] string accessToken) =>
            {
                bool esValido = tokenRepo.ValidarToken(accessToken);
                if (!esValido)
                {

                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                }
                   

                return Results.Ok(true);
            })
            .WithName("ValidateToken")
            .WithOpenApi();

        }

        #endregion



    }
}
