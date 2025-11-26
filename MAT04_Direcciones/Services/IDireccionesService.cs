using MySqlX.XDevAPI.Common;

namespace MAT04_Direcciones.Services
{
    public interface IDireccionesService
    {
        Task<IEnumerable<Entities.Provincias>> Obtener_Todos_Provincias();
        Task<(IEnumerable<Entities.cantones> cantones, string mensaje)> Obtener_Cantones_Por_Provincia(string provincia);

        Task<(IEnumerable<Entities.distritos> distritos, string mensaje)> Obtener_Distritos_Por_Canton_Provincia(string provincia, string canton);


    }
}
