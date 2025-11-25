using System.Text.Json.Serialization;

namespace Módulo_de_Oferta_académica_ACD6.Entities
{
    public class Profesor
    {
        public string ID_Profesor { get; set; } = null!;
        public string TipoIdentificacion { get; set; } = null!;
        public string NumeroIdentificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; } = null!;

        [JsonIgnore]
        public string? Accion { get; set; }

        [JsonIgnore]
        public string? Mensaje { get; set; }
    }
}
