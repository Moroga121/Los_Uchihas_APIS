using System.Text.Json.Serialization;

namespace Notificaciones.Entities
{
    public class Notificacion
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Cuerpo { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime FechaEnvio { get; set; }

    }

    public class ListaNotificaciones
    {
        public DateTime FechaEnvio { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

    }
}
