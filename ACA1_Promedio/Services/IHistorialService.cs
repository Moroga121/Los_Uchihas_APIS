using ACA1_Promedio.Entities;

namespace ACA1_Promedio.Services
{
    public interface IHistorialService
    {
        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
        Task<IEnumerable<HistorialAcademico>> ObtenerHistorialAsync(string tipoIdentificacion, string numeroIdentificacion, int? año, string? periodo);
    }
}
