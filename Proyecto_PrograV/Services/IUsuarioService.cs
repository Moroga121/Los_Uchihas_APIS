using Proyecto_PrograV.Entities;

namespace Proyecto_PrograV.Services
{
    public interface IUsuarioService
    {

        #region "Get Usuarios"

        // Obtener todos los Usuarios
        Task<IEnumerable<Usuario>> Obtener_Todos_Los_Usuarios();

        Task<(Usuario usuario, string mensaje)> Obtener_Usuario_Por_Identificacion(string identificacion);

        // Obtener usuario por ID
        Task<(Usuario usuario, string mensaje)> Obtener_Usuario_Por_ID(string identificacion);

        // Obtener Usuarios Filtrados
        Task<IResult> Obtener_Usuarios_FiltradosAsync(string identificacion, string nombre, string rol, string tipo, string dominio);

        // Obtener Tipos de Identificación
        Task<IEnumerable<Tipos_Identificacion>> Obtener_Tipos_De_Identificacion();

        // Obtener Dominios

        Task<IEnumerable<Usuario>> Obtener_Dominios();

        // Cambiar Contraseña

        Task<IResult> Cambiar_Contrasena_UsuarioAsync(Usuario usu);


        #endregion

        #region "CRUD Usuarios"

        // Crud Usuarios

        Task<IResult> CRUD_UsuariosAsync(Usuario usuario);

        #endregion

        #region Registro bitacora
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);
        #endregion


    }
}
