using Módulo_de_Ofertas_académica_ACD2.Entities;

namespace Módulo_de_Ofertas_académica_ACD2.Services
{
    public interface ICarreraService
    {
        Task<IEnumerable<Carrera>> Obtener_Todas_Las_Carreras();
        Task<(Carrera carrera, string mensaje)> Obtener_Carrera_Por_ID(string idCarrera);
        Task<IEnumerable<Carrera>> Obtener_Carreras_Por_Institucion(string idInstitucion);
        Task<IResult> CRUD_CarrerasAsync(Carrera carrera);
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
    }
}
