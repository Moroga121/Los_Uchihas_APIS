using Módulo_de_Oferta_académica_ACD4.Entities;

namespace Módulo_de_Oferta_académica_ACD4.Sercives
{
    public interface IGrupoService
    {
        Task<IEnumerable<Grupo>> Obtener_Todos_Los_Grupos();
        Task<(Grupo grupo, string mensaje)> Obtener_Grupo_Por_ID(string idGrupo);
        Task<IResult> CRUD_GruposAsync(Grupo grupo);
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
    }
}
