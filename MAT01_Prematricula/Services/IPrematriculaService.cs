namespace MAT01_Prematricula.Services
{
    public interface IPrematriculaService
    {
        Task<IEnumerable<Entities.Prematricula>> Obtener_Todas_Prematriculas();
        Task<(Entities.Prematricula prematricula, string mensaje)> Obtener_Prematricula_Por_ID(string Id_Prematricula);

        Task<IResult> CRUDPrematricula(Entities.Prematricula prematricula);
    }
}
