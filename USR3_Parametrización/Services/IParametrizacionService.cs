using USR3_Parametrización.Entities;

namespace USR3_Parametrización.Services
{
    public interface IParametrizacionService
    {

        #region "Get Roles"

        // Obtener todos los Roles

        Task<IEnumerable<Parametrizacion>> Obtener_Todos_Los_Parametros();

        // Obtener Rol por ID

        Task<(Parametrizacion parametro, string mensaje)> Obtener_Parametro_Por_ID(string indetificador_parametro);



        #endregion

        #region "CRUD Parametrización"

        // Crud Parametrización

        Task<IResult> CRUD_ParametrizacionAsync(Parametrizacion parametrizacion);

        #endregion

        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion

    }
}
