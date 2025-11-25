using Módulo_de_Oferta_académica_ACD5.Entities;

namespace Módulo_de_Oferta_académica_ACD5.Service
{
    public interface IPeriodoService
    {
        Task<IEnumerable<Periodo>> Obtener_Todos_Los_Periodos();
        Task<(Periodo periodo, string mensaje)> Obtener_Periodo_Por_ID(string idPeriodo);
        Task<IResult> CRUD_PeriodosAsync(Periodo periodo);
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
    }
}
