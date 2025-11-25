using System.Text.Json.Serialization;

namespace Proyecto_PrograV.Entities
{
    public class Usuario
    {

        public string Identificacion { get; set; } = null!;

        public string Tipo_Identificacion { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public string Rol_Usuario { get; set; } = null!;

        [JsonIgnore]
        public string Mensaje { get; set; } = null!;

        [JsonIgnore]
        public string Accion { get; set; } = null!;


    }
}
