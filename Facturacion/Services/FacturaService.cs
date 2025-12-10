using System.Net.Http;
using System.Text.Json;
using Facturacion.Entities;
using Facturacion.Repository;

namespace Facturacion.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly FacturaRepository _facturaRepository;
        private readonly HttpClient _httpClient = new HttpClient();
        public FacturaService(FacturaRepository facturaRepository, HttpClient httpClient)
        {
            _facturaRepository = facturaRepository;
            _httpClient = httpClient;
        }

        #region "Registrar Bitácora"


        public async Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/GEN01Bitacora/bitacora/registrar");

            // Agregar token al header
            request.Headers.Add("access_token", accessToken);

            // Crear el JSON a enviar
            var body = new
            {
                Accion = accion,
                Descripcion = descripcion
            };

            // Serializar a JSON
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request, ct);
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Bitácora registrada exitosamente");
                }
                else
                {
                    var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>(cancellationToken: ct);
                    if (payload != null && payload.TryGetValue("mensaje", out var m))
                    {
                        return (false, m ?? "Error desconocido al registrar bitácora");
                    }
                    return (false, "Error desconocido al registrar bitácora");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error al procesar la respuesta de la API: {ex.Message}");
            }
        }


        #endregion

        public async Task<(bool Exito, string Mensaje, Factura? FacturaCreada)> CrearFacturaAsync(string idEstudiante, int montoBase, string periodo)
        {
            if (string.IsNullOrWhiteSpace(idEstudiante))
                return (false, "Debe indicar la identificación del estudiante.", null);

            if (montoBase <= 0)
                return (false, "El monto base debe ser mayor que cero.", null);

            if (string.IsNullOrWhiteSpace(periodo))
                return (false, "Debe indicar el periodo.", null);

            try
            {
                decimal iva = montoBase * 0.02m;
                decimal total = montoBase + iva;
                string detalle = "Servicios estudiantiles";

                var facturaCreada = await _facturaRepository.CrearFacturaAsync(idEstudiante, montoBase, iva, total, detalle, periodo);

                if (facturaCreada == null)
                    return (false, "No se pudo crear la factura.", null);

                return (true, "Factura creada correctamente.", facturaCreada);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear la factura: {ex.Message}", null);
            }
        }


        public async Task<(bool Exito, string Mensaje, Factura? Factura)> ObtenerFacturaPorNumeroAsync(long numeroFactura)
        {
            if (numeroFactura <= 0)
                return (false, "El número de factura debe ser mayor que cero.", null);

            try
            {
                var factura = await _facturaRepository.ObtenerFacturaPorNumeroAsync(numeroFactura);

                if (factura is null)
                    return (false, $"No se encontró la factura con número {numeroFactura}.", null);

                return (true, "Factura encontrada correctamente.", factura);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar la factura: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, IEnumerable<Factura>? Facturas)> ObtenerFacturasPorPeriodoAsync(string periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return (false, "Debe indicar el periodo a consultar.", null);

            try
            {
                var facturas = await _facturaRepository.ObtenerFacturasPorPeriodoAsync(periodo);

                if (facturas == null || !facturas.Any())
                    return (false, $"No se encontraron facturas para el periodo {periodo}.", null);

                return (true, "Facturas obtenidas correctamente.", facturas);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar las facturas: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, IEnumerable<Detalle>? detalles)> ObtenerEncabezadoDetalleAsync(long idFactura)
        {
            try
            {
                var facturas = await _facturaRepository.ObtenerEncabezadoDetalleAsync(idFactura);

                if (facturas == null || !facturas.Any())
                    return (false, $"No se encontraron detalles para la factura {idFactura}.", null);

                return (true, "Detalles obtenidos correctamente.", facturas);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar los detalles: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, IEnumerable<Factura>? Facturas)> ObtenerFacturas()
        {
            try
            {
                var facturas = await _facturaRepository.ObtenerFacturas();

                if (facturas == null || !facturas.Any())
                    return (false, $"No se encontraron facturas.", null);

                return (true, "Facturas obtenidas correctamente.", facturas);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar las facturas: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, Factura? Factura)> ReversarFacturaAsync(long numeroFactura, string motivo)
        {
            // 🔹 Validación 1: número válido
            if (numeroFactura <= 0)
            {
                return (false, "El número de factura debe ser mayor que cero.", null);
            }

            var factura = await _facturaRepository.ObtenerFacturaPorNumeroAsync(numeroFactura);

            if (factura == null)
            {
                return (false, $"No se encontró la factura número {numeroFactura}.", null);
            }

            // 🔹 Validación 2: estado de factura
            if (factura.Estado_Factura == "Anulada")
            {
                return (false, "La factura ya está anulada.", factura);
            }

            try
            {
                var facturaReversada = await _facturaRepository.ReversarFacturaAsync(numeroFactura, motivo);
                return (true, "Factura anulada correctamente.", facturaReversada);
            }
            catch (Exception ex)
            {
                return (false, $"Error al anular la factura: {ex.Message}", null);
            }
        }
    }


}

