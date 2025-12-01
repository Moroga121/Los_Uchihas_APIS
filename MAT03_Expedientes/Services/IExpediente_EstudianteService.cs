namespace MAT03_Expedientes.Services
{
    public interface IExpediente_EstudianteService
    {
        Task<IResult> CRUDExpediente(Entities.Expediente_Estudiantes expediente_Estudiantes);
        Task<IEnumerable<Entities.Expediente_Estudiantes>> Obtener_Todos_Expedientes();
        Task<(Entities.Expediente_Estudiantes expediente_Estudiantes, string mensaje)> Obtener_Expediente_Por_ID(string numero_identificacion);
        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
    }
}
