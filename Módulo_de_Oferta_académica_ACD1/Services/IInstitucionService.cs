using Módulo_de_Oferta_académica_ACD1.Entities;

namespace Módulo_de_Oferta_académica_ACD1.Services
{
    public interface IInstitucionService
    {
        Task<IEnumerable<Institucion>> Obtener_Todas_Las_Instituciones();
        Task<(Institucion institucion, string mensaje)> Obtener_Institucion_Por_ID(string idInstitucion);
        Task<IResult> CRUD_InstitucionesAsync(Institucion institucion);
        Task<IEnumerable<Institucion>> Buscar_Instituciones_Por_Nombre(string nombre);
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

    }
}
