using System.Net.Http;
using System.Text.Json;
using Pagos.Entities;
using Pagos.Repository;

namespace Pagos.Services
{
    public class PagosService : IPagosService
    {
        private readonly PagosRepository _pagoRepository;
        private readonly HttpClient _httpClient = new HttpClient();
        public PagosService(PagosRepository pagoRepository, HttpClient httpClient)
        {
            _pagoRepository = pagoRepository;
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

        public async Task<(bool Exito, string Mensaje, Pago? PagoCreado)> CrearPagoFacturaAsync(long id_Factura, decimal montoPago, string url)
        {
            if (id_Factura <= 0)
                return (false, "Debe indicar un número de factura válido.",null);

            try
            {
               var pagoCreado = await _pagoRepository.CrearPagoFacturaAsync(id_Factura, montoPago, url);

                return (true, "Pago de factura creado correctamente.", pagoCreado);
            }
            catch (Exception ex)
            {
                return (false, $"{ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, Pago? Pago)> ObtenerPagoFacturaPorNumeroAsync(int numeroPago)
        {
            if (numeroPago <= 0)
                return (false, "El número de pago debe ser mayor que cero.", null);

            try
            {
                var pago = await _pagoRepository.ObtenerPagoFacturaPorNumeroAsync(numeroPago);

                if (pago is null)
                    return (false, $"No se encontró el pago número {numeroPago}.", null);

                return (true, "Pago encontrado correctamente.", pago);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar el pago: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, BuscarFacturaDto? Pago)> ObtenerFacturaPorNumeroAsync(int numerofactura)
        {

            try
            {
                var pago = await _pagoRepository.ObtenerFacturaPorNumeroAsync(numerofactura);

                if (pago is null)
                    return (false, $"No se encontró la factura número {numerofactura}.", null);

                return (true, "Factura encontrada correctamente.", pago);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar la factura: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, IEnumerable<Pago>? Pago)> ObtenerPagoFacturaAsync()
        {
            try
            {
                var pago = await _pagoRepository.ObtenerPagoFacturaAsync();

                if (pago is null)
                    return (false, $"No se encontraron pagos.", null);

                return (true, "Pagos encontrados correctamente.", pago);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar el pago: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, IEnumerable<Pago>? Pagos)> ObtenerPagoFacturasPorPeriodoAsync(string periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return (false, "Debe indicar el periodo a consultar.", null);

            try
            {
                var pagos = await _pagoRepository.ObtenerPagoFacturasPorPeriodoAsync(periodo);

                if (pagos == null || !pagos.Any())
                    return (false, $"No se encontraron pagos para el periodo {periodo}.", null);

                return (true, "Pagos obtenidos correctamente.", pagos);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar los pagos: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje, Pago? Pago)> ReversarPagoAsync(int numeroPago, string motivo)
        {
            if (numeroPago <= 0)
                return (false, "Debe indicar un número de pago válido.", null);

            try
            {
                var pago = await _pagoRepository.ObtenerPagoFacturaPorNumeroAsync(numeroPago);

                if (pago == null)
                    return (false, $"No se encontró el pago número {numeroPago}.", null);

                if (pago.Estado_Pago == "Anulado")
                    return (false, "El pago ya se encuentra anulado.", pago);

                var pagoReversado = await _pagoRepository.ReversarPagoAsync(numeroPago, motivo);
                return (true, "Pago anulado correctamente.", pagoReversado);
            }
            catch (Exception ex)
            {
                return (false, $"{ex.Message}", null);
            }
        }
    }
}
