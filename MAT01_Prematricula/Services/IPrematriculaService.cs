namespace MAT01_Prematricula.Services
{
    public interface IPrematriculaService
    {
        Task<IEnumerable<Entities.Prematricula>> Obtener_Todas_Prematriculas();
        Task<(Entities.Prematricula prematricula, string mensaje)> Obtener_Prematricula_Por_ID(string Id_Prematricula);

        Task<IEnumerable<Entities.Prematricula>> Obtener_Prematricula_Por_Identificacion(string numero_identificacion);

        Task<IResult> CRUDPrematricula(Entities.Prematricula prematricula);
        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
    }
}
