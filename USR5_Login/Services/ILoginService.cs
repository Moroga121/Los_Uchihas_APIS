using USR5_Login.Entities;

namespace USR5_Login.Services
{
    public interface ILoginService
    {

        // Validar Usuario
        Task<IResult> ValidarUsuarioAsync(Login login);

        #region bitacora

        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string email, string accion, object descripcion, CancellationToken ct = default);

        #endregion
    }
}
