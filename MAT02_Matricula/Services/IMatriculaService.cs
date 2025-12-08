namespace MAT02_Matricula.Services
{
    public interface IMatriculaService
    {
        Task<IEnumerable<Entities.MatriculaCompleta>> Obtener_Todas_Matriculas();
        Task<IResult> CRUDMatricula(Entities.Matricula matricula);
        Task<IEnumerable<Entities.MatriculaCompleta>> Obtener_Matriculados_Por_Curso_Grupo(string curso, string grupo);
        #region bitacora

        Task<IEnumerable<Entities.Matricula>> Obtener_Matricula_Por_Identificacion(string identificacion);

        #region Registro bitacora
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
        #endregion

    }
}
