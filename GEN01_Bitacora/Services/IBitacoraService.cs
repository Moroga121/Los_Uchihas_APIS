using GEN01_Bitacora.Entities;

namespace GEN01_Bitacora.Services
{
    public interface IBitacoraService
    {
        Task Registrar_Bitacora(string usuario, string accion, object descripcion);

        Task<IEnumerable<Bitacora>> Obtener_Todas_Las_Bitacoras();

        Task<IEnumerable<Bitacora>> Obtener_Todas_Las_BitacorasFiltradas(DateOnly? fechaInicio, DateOnly? fechaFin, string? accion, string? usuario);

    }
}
