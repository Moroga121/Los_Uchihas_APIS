using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pagos.Entities;
using Pagos.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pagos
{
    public static class PagosEndpoint
    {
        public static void MapPagoFacturaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/pago").WithTags(nameof(Pago));

            #region Crear Pago de Factura
            group.MapPost("/", async (
                [FromHeader(Name = "access_token")] string accessToken,
                HttpClient httpClient,
                [FromServices] IPagosService pagoService,
                [FromBody] CrearPagoRequest request) =>
            {
                var validationRequest = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                validationRequest.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(validationRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultado = await pagoService.CrearPagoFacturaAsync(
                    request.Numero_Factura,
                    request.Monto,
                    request.Ruta_Comprobante
                );

                await pagoService.RegistrarBitacoraAsync(
                   accion: "Crear pago factura",
                   descripcion: resultado.PagoCreado,
                   accessToken: accessToken
                );

                if (!resultado.Exito)
                    return Results.BadRequest(new { mensaje = resultado.Mensaje });

                return Results.Created($"/pago/{resultado.PagoCreado.Numero_Pago}", resultado.PagoCreado);
            })
            .WithName("CrearPagoFactura")
            .WithOpenApi();
            #endregion


            #region Obtener Pago por Número
            group.MapGet("/{numero}", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IPagosService pagoService, int numero) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultado = await pagoService.ObtenerPagoFacturaPorNumeroAsync(numero);

                await pagoService.RegistrarBitacoraAsync(
                   accion: "Obtener pago ID",
                   descripcion: resultado.Pago,
                   accessToken: accessToken
                );

                if (!resultado.Exito)
                    return Results.NotFound(new { mensaje = resultado.Mensaje });

                return Results.Ok(resultado.Pago);
            })
            .WithName("ObtenerPagoFacturaPorNumero")
            .WithOpenApi();
            #endregion

            #region Obtener Pago por Número
            group.MapGet("/f/{numero}", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IPagosService pagoService, int numero) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultado = await pagoService.ObtenerFacturaPorNumeroAsync(numero);

                if (!resultado.Exito)
                    return Results.NotFound(new { mensaje = resultado.Mensaje });

                return Results.Ok(resultado.Pago);
            })
            .WithName("ObtenerFacturaPorNumero")
            .WithOpenApi();
            #endregion

            #region Obtener Pago por Número
            group.MapGet("/a/", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IPagosService pagoService) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultado = await pagoService.ObtenerPagoFacturaAsync();

                await pagoService.RegistrarBitacoraAsync(
                   accion: "Obtener pago factura",
                   descripcion: resultado.Pago,
                   accessToken: accessToken
                );
                if (!resultado.Exito)
                    return Results.NotFound(new { mensaje = resultado.Mensaje });

                return Results.Ok(resultado.Pago);
            })
            .WithName("ObtenerPagoFactura")
            .WithOpenApi();
            #endregion

            #region Listar Pagos por Periodo
            group.MapGet("/", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IPagosService pagoService, [FromQuery] string periodo) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultado = await pagoService.ObtenerPagoFacturasPorPeriodoAsync(periodo);

                await pagoService.RegistrarBitacoraAsync(
                   accion: "Obtener pagos periodo",
                   descripcion: resultado.Pagos,
                   accessToken: accessToken
                );

                if (!resultado.Exito)
                    return Results.BadRequest(new { mensaje = resultado.Mensaje });

                return Results.Ok(resultado.Pagos);
            })
            .WithName("ObtenerPagoFacturasPorPeriodo")
            .WithOpenApi();
            #endregion

            #region Anular Pago de Factura
            group.MapPatch("/", async (
                [FromHeader(Name = "access_token")] string accessToken,
                [FromBody] ReversarPagoRequest request,
                HttpClient httpClient,
                [FromServices] IPagosService pagoService) =>
            {
                var validationRequest = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                validationRequest.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(validationRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                // Validar que se proporcione el motivo
                if (string.IsNullOrWhiteSpace(request.Motivo_Reversa))
                {
                    return Results.BadRequest(new { mensaje = "El motivo de la reversión es obligatorio" });
                }


                var pagoAntes = await pagoService.ObtenerPagoFacturaPorNumeroAsync(request.Numero_Pago);

                var resultado = await pagoService.ReversarPagoAsync(request.Numero_Pago, request.Motivo_Reversa);

                var pagoDespues = await pagoService.ObtenerPagoFacturaPorNumeroAsync(request.Numero_Pago);

                var antesYDespues = new
                {
                    Antes = pagoAntes.Pago,
                    Despues = pagoDespues.Pago
                };

                string descripcionJson = JsonSerializer.Serialize(antesYDespues);

                await pagoService.RegistrarBitacoraAsync(
                   accion: "Reversar Pago",
                   descripcion: descripcionJson,
                   accessToken: accessToken
                );


                if (!resultado.Exito)
                    return Results.BadRequest(new { mensaje = resultado.Mensaje });

                return Results.Ok(resultado.Pago);
            })
            .WithName("AnularPagoFactura")
            .WithOpenApi();
            #endregion
        }
    }
}