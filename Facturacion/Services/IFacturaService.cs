using Facturacion.Entities;

namespace Facturacion.Services
{
    public interface IFacturaService
    {
        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
        Task<(bool Exito, string Mensaje, Factura? FacturaCreada)> CrearFacturaAsync(string idEstudiante, int montoBase, string periodo);
        Task<(bool Exito, string Mensaje, Factura? Factura)> ObtenerFacturaPorNumeroAsync(long numeroFactura);
        Task<(bool Exito, string Mensaje, IEnumerable<Factura>? Facturas)> ObtenerFacturasPorPeriodoAsync(string periodo);
        Task<(bool Exito, string Mensaje, IEnumerable<Factura>? Facturas)> ObtenerFacturas();
        Task<(bool Exito, string Mensaje, Factura? Factura)> ReversarFacturaAsync(long numeroFactura, string motivo);
        Task<(bool Exito, string Mensaje, IEnumerable<Detalle>? detalles)> ObtenerEncabezadoDetalleAsync(long idFactura);

    }

}
