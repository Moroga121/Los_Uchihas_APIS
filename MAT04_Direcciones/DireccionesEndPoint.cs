using MAT04_Direcciones.Entities;
using MAT04_Direcciones.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAT04_Direcciones
{
    public static class DireccionesEndPoint
    {
        public static void MapProvinciasEndPoints (this WebApplication app)
        {
            app.MapGet("/provincias", async (Services.IDireccionesService direccionesService, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Unauthorized();

                }
                var provincias = await direccionesService.Obtener_Todos_Provincias();
                // Registrar intento exitoso en la bitacora del login
                await direccionesService.RegistrarBitacoraAsync(
                   accion: "Obtener todas las provincias",
                   descripcion: provincias,
                   accessToken: accessToken
               );
                return Results.Ok(provincias);
            })
            .WithName("GetProvincias")
            .Produces<IEnumerable<Entities.Provincias>>(StatusCodes.Status200OK)
            .WithTags("Direcciones");
        }
        public static void MapCantonesEndPoints(this WebApplication app)
        {
            app.MapGet("/cantones", async (string provincia, IDireccionesService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Unauthorized();

                }
                var (cantones, mensaje) = await service.Obtener_Cantones_Por_Provincia(provincia);

                if (mensaje.Contains("Cantones obtenidos correctamente", StringComparison.OrdinalIgnoreCase))
                {
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener cantones por provincia",
                       descripcion: cantones,
                       accessToken: accessToken
                   );
                    return Results.Ok(new { mensaje, cantones });
                }

                return Results.BadRequest(new { mensaje });
            });

        }
        public static void MapDistritosEndPoints(this WebApplication app)
        {
            app.MapGet("/distritos", async (string provincia, string canton, IDireccionesService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    return Results.Unauthorized();

                }
                var (distritos, mensaje) = await service.Obtener_Distritos_Por_Canton_Provincia(provincia , canton);

                if (mensaje.Contains("Distritos obtenidos correctamente", StringComparison.OrdinalIgnoreCase))
                {
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener distritos por canton y provincia",
                       descripcion: distritos,
                       accessToken: accessToken
                   );
                    return Results.Ok(new { mensaje, distritos });
                }

                return Results.BadRequest(new { mensaje });    
            });

        }
    }
}
