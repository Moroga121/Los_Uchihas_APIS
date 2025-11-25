using Módulo_de_Oferta_académica_ACD6.Entities;

namespace Módulo_de_Oferta_académica_ACD6.Services
{
    public interface IProfesorService
    {
        Task<IEnumerable<Profesor>> Obtener_Todos();
        Task<(Profesor? profesor, string mensaje)> Obtener_Por_ID(string id);
        Task<IResult> CRUD_ProfesoresAsync(Profesor profesor);
        Task<IEnumerable<Profesor>> BuscarProfesoresAsync(string busqueda, string ordenCampo, string ordenDireccion, int pagina, int tamanoPagina);
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
    }
}

