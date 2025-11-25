using USR2_Roles.Entities;

namespace USR2_Roles.Services
{
    public interface IRolService
    {

        #region "Get Roles"

        // Obtener todos los Roles

        Task<IEnumerable<Rol>> Obtener_Todos_Los_Roles();

        // Obtener Rol por ID
        Task<(Rol rol, string mensaje)> Obtener_Rol_Por_ID(string identificador_rol);



        #endregion

        #region "CRUD Roles"

        // Crud Roles

        Task<IResult> CRUD_RolesAsync(Rol usuario);

        #endregion

        #region Actualizar permisos con modulos
        Task<IResult> ActualizarPermisosAsync(string rolId, List<string> modulos);
        #endregion

        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
    }
}
