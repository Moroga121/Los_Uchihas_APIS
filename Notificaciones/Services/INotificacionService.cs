using Notificaciones.Entities;

namespace Notificaciones.Services
{
    public interface INotificacionService
    {
        Task<(bool Success, string Message)> CrearYEnviarNotificacionAsync(Notificacion notificacion);
        Task<List<ListaNotificaciones>> ObtenerNotificacionesAsync();
        Task<List<ListaNotificaciones>> ObtenerNotificacionesPorEmailAsync(string email);

    }
}

