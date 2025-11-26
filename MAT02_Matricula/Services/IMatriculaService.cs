namespace MAT02_Matricula.Services
{
    public interface IMatriculaService
    {
        Task<IEnumerable<Entities.MatriculaCompleta>> Obtener_Todas_Matriculas();
        Task<IResult> CRUDMatricula(Entities.Matricula matricula);
        Task<IEnumerable<Entities.MatriculaCompleta>> Obtener_Matriculados_Por_Curso_Grupo(string curso, string grupo);

    }
}
