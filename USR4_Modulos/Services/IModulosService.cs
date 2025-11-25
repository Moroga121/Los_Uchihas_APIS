using System.Collections.Generic;
using USR4_Modulos.Entities;

namespace USR4_Modulos.Services
{
    public interface IModulosService
    {

        #region "Get Modulo"

        // Obtener todos los Modulos

        Task<IEnumerable<Modulos>> Obtener_Todos_Los_Modulos();

        // Obtener Módulo por ID

        Task<(Modulos modulo, string mensaje)> Obtener_Modulo_Por_ID(string indetificador_modulo);

        // Obtener Módulo por Rol
        Task<(IEnumerable<Modulos>, string mensaje)> Obtener_Modulo_Por_Rol(string rol);

        // Obtener todos los roles y modulos relacionados
        Task<IEnumerable<Rol_Modulo>> Obtener_Todos_Los_Rol_Usuario();



        #endregion

        #region "CRUD Modulo"

        // Crud Parametrización

        Task<IResult> CRUD_ModulosAsync(Modulos modulo);

        #endregion

        Task<IResult> ActualizarPermisosAsync(string moduloId, List<string> rolesAsignados);

        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

        #endregion
    }
}
