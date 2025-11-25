using Módulo_de_Oferta_académica_ACD3.Entities;

namespace Módulo_de_Oferta_académica_ACD3.Services
{
    public interface ICursoService
    {
        Task<IEnumerable<Curso>> Obtener_Todos_Los_Cursos();
        Task<(Curso curso, string mensaje)> Obtener_Curso_Por_ID(string idCurso);
        Task<IEnumerable<Curso>> Obtener_Cursos_Por_Carrera(string idCarrera);
        Task<IResult> CRUD_CursosAsync(Curso curso);
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
    }
}
