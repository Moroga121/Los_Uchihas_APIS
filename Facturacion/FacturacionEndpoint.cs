using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using Facturacion.Entities;
using Facturacion.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Facturacion
{
    public static class FacturacionEndpoint
    {
        public static void MapFacturaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/factura").WithTags(nameof(Factura));

            #region "Crear factura desde matrícula"

            group.MapPost("/", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IFacturaService facturaService, [FromBody] Factura factura) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var resultado = await facturaService.CrearFacturaAsync(factura.Identificacion, factura.MontoBase, factura.Periodo);

                await facturaService.RegistrarBitacoraAsync(
                   accion: "Crear factura",
                   descripcion: resultado.FacturaCreada,
                   accessToken: accessToken
                );

                if (!resultado.Exito)
                    return Results.BadRequest(new { mensaje = resultado.Mensaje });

                return Results.Created($"/factura/{resultado.FacturaCreada.Numero_Factura}", resultado.FacturaCreada);
            })
            .WithName("CrearFactura")
            .WithOpenApi();

            #endregion

            #region "Obtener factura por número"
            group.MapGet("/{numero}", async ([FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient, [FromServices] IFacturaService facturaService, long numero) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                var factura = await facturaService.ObtenerFacturaPorNumeroAsync(numero);

                await facturaService.RegistrarBitacoraAsync(
                   accion: "Obtener Factura ID",
                   descripcion: factura.Factura,
                   accessToken: accessToken
                );

                if (!factura.Exito)
                    return Results.BadRequest(new { mensaje = factura.Mensaje });
                return Results.Ok(factura.Factura);
            })
            .WithName("ObtenerFacturaPorNumero")
            .WithOpenApi();

            #endregion

            #region "Obtener factura por periodo"
            group.MapGet("/", async ([FromHeader(Name = "access_token")] string accessToken,
                                     HttpClient httpClient,
                                     [FromServices] IFacturaService facturaService,
                                     [FromQuery] string? periodo) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                var facturas = string.IsNullOrEmpty(periodo)
                    ? await facturaService.ObtenerFacturas()
                    : await facturaService.ObtenerFacturasPorPeriodoAsync(periodo);

                await facturaService.RegistrarBitacoraAsync(
                   accion: "Obtener Factura Periodo",
                   descripcion: facturas.Facturas,
                   accessToken: accessToken
                );

                if (!facturas.Exito)
                    return Results.BadRequest(new { mensaje = facturas.Mensaje });

                return Results.Ok(facturas.Facturas);
            })
            .WithName("ObtenerFacturas")
            .WithOpenApi();
            #endregion

            #region "Reversar factura"

            group.MapPatch("/", async ([FromHeader(Name = "access_token")] string accessToken,HttpClient httpClient,[FromServices] IFacturaService facturaService,[FromBody] ReversarFactura request) =>
            {
                // Validación del token
                var requestValidate = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                requestValidate.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(requestValidate);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);
                }

                // Validación del request
                if (request == null)
                {
                    return Results.BadRequest(new { mensaje = "Datos requeridos" });
                }

                if (request.NumeroFactura <= 0)
                {
                    return Results.BadRequest(new { mensaje = "El número de factura debe ser mayor que cero" });
                }

                if (string.IsNullOrWhiteSpace(request.Motivo))
                {
                    return Results.BadRequest(new { mensaje = "El motivo es requerido" });
                }

                var facturaAntes = await facturaService.ObtenerFacturaPorNumeroAsync(request.NumeroFactura);

                var resultado = await facturaService.ReversarFacturaAsync(request.NumeroFactura, request.Motivo);

                var facturaDespues = await facturaService.ObtenerFacturaPorNumeroAsync(request.NumeroFactura);

                var antesYDespues = new
                {
                    Antes = facturaAntes.Factura,
                    Despues = facturaDespues.Factura
                };

                string descripcionJson = JsonSerializer.Serialize(antesYDespues);

                await facturaService.RegistrarBitacoraAsync(
                   accion: "Reversar Factura",
                   descripcion: descripcionJson,
                   accessToken: accessToken
                );
                if (!resultado.Exito)
                {
                    if (resultado.Mensaje.Contains("no se encontró", StringComparison.OrdinalIgnoreCase))
                        return Results.NotFound(new { mensaje = resultado.Mensaje });

                    if (resultado.Mensaje.Contains("anulada", StringComparison.OrdinalIgnoreCase))
                        return Results.Conflict(new { mensaje = resultado.Mensaje });

                    return Results.Problem(resultado.Mensaje);
                }

                return Results.Ok(new
                {
                    mensaje = "Factura anulada correctamente",
                    factura = resultado.Factura
                });
            })
            .WithName("ReversarFactura")
            .WithOpenApi();

            #endregion

            #region "Obtener detalle encabezado por número"
            group.MapGet("/d/", async ([FromHeader(Name = "access_token")] string accessToken,
                                     HttpClient httpClient,
                                     [FromServices] IFacturaService facturaService,
                                     [FromQuery] long idFactura) =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                request.Headers.Add("access_token", accessToken);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return Results.Json(new { mensaje = "No autorizado" }, statusCode: 401);

                var facturas = await facturaService.ObtenerEncabezadoDetalleAsync(idFactura);

                await facturaService.RegistrarBitacoraAsync(
                   accion: "Obtener Encabezado - Detale",
                   descripcion: facturas.detalles,
                   accessToken: accessToken
                );

                if (!facturas.Exito)
                    return Results.BadRequest(new { mensaje = facturas.Mensaje });

                return Results.Ok(facturas.detalles);
            })
            .WithName("ObtenerEncabezado")
            .WithOpenApi();
            #endregion
        }
    }
}
