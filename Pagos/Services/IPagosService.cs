using System.Net.Http;
using System.Text.Json;
using Pagos.Entities;
namespace Pagos.Services
{
    public interface IPagosService
    {
        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
        Task<(bool Exito, string Mensaje, Pago? PagoCreado)> CrearPagoFacturaAsync(long id_Factura, decimal montoPago, string url);
        Task<(bool Exito, string Mensaje, Pago? Pago)> ObtenerPagoFacturaPorNumeroAsync(int numeroPago);
        Task<(bool Exito, string Mensaje, IEnumerable<Pago>? Pago)> ObtenerPagoFacturaAsync();
        Task<(bool Exito, string Mensaje, IEnumerable<Pago>? Pagos)> ObtenerPagoFacturasPorPeriodoAsync(string periodo);
        Task<(bool Exito, string Mensaje, BuscarFacturaDto? Pago)> ObtenerFacturaPorNumeroAsync(int numerofactura);
        Task<(bool Exito, string Mensaje, Pago? Pago)> ReversarPagoAsync(int numeroPago, string motivo);
    }
}
