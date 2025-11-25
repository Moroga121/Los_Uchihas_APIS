using System.Text.Json.Serialization;

namespace USR4_Modulos.Entities
{
    public class Modulos
    {

        public string Identificador_Modulo { get; set; } = null!;

        public string Nombre_Modulo { get; set; } = null!;
        public string Estado { get; set; }
        public int Orden { get; set; }

        [JsonIgnore]
        public string Mensaje { get; set; } = null!;

        [JsonIgnore]
        public string Accion { get; set; } = null!;
    }
    public class Rol_Modulo
    {
        public int ID_Rol_Usuario { get; set; }
        [JsonPropertyName("Identificador_Rol")]
        public string Identificador_Rol { get; set; } = null!;
        [JsonPropertyName("Identificador_Modulo")]
        public string Identificador_Modulo { get; set; } = null!;

        public List<string> Roles { get; set; } = new();
    }
}
