using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using Notificaciones.Entities;
using Notificaciones.Repository;

namespace Notificaciones.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly NotificacionRepository _notificacionRepository;
        private readonly EmailHelper _emailHelper;

        public NotificacionService(NotificacionRepository notificacionRepository, EmailHelper emailHelper)
        {
            _notificacionRepository = notificacionRepository;
            _emailHelper = emailHelper;
        }

        public async Task<(bool Success, string Message)> CrearYEnviarNotificacionAsync(Notificacion notificacion)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(notificacion.Email))
                throw new ArgumentException("El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(notificacion.Asunto))
                throw new ArgumentException("El asunto es obligatorio.");

            if (string.IsNullOrWhiteSpace(notificacion.Cuerpo))
                throw new ArgumentException("El cuerpo es obligatorio.");

            try
            {
                // Usar el método que soporta múltiples correos
                var resultadoEnvio = await _emailHelper.EnviarCorreoMultipleAsync(
                    notificacion.Email,
                    notificacion.Asunto,
                    notificacion.Cuerpo);

                // Guardar en BD con estado
                string estado = resultadoEnvio.Success ? "E" : "F";

                await _notificacionRepository.CrearNotificacionAsync(
                    notificacion.Email,
                    notificacion.Asunto,
                    notificacion.Cuerpo,
                    estado);

                return resultadoEnvio;
            }
            catch (Exception ex)
            {
                // Guardar en BD con estado de error
                await _notificacionRepository.CrearNotificacionAsync(
                    notificacion.Email,
                    notificacion.Asunto,
                    notificacion.Cuerpo,
                    "F"); // F=Fallido

                return (false, $"Error al procesar: {ex.Message}");
            }
        }
        public async Task<List<ListaNotificaciones>> ObtenerNotificacionesAsync()
        {
            return await _notificacionRepository.ObtenerNotificacionesAsync();
        }

        public async Task<List<ListaNotificaciones>> ObtenerNotificacionesPorEmailAsync(string email)
        {
            
            return await _notificacionRepository.ObtenerNotificacionesPorEmailAsync(email);

        }

    }

}