using System.Text.Json.Serialization;

namespace MAT03_Expedientes.Entities
{
    public class Expediente_Estudiantes
    {
        public string? numero_identificacion { get; set; }
        public string? tipo_identificacion { get; set; }
        public string? email { get; set; }
        public string? nombre { get; set; }
        //public string? primer_apellido { get; set; }
        //public string? segundo_apellido { get; set; }
        public DateTime? fecha_nacimiento { get; set; }
        public int? id_distrito { get; set; }
        public string? otras_senas { get; set; }
        public string? telefono { get; set; }
        [JsonIgnore]
        public string? Accion { get; set; } = null!;

    }

    public class Usuario
    {
        [JsonPropertyName("identificacion")]
        public string Identificacion { get; set; }

        [JsonPropertyName("tipo_Identificacion")]
        public string Tipo_Identificacion { get; set; }

        [JsonPropertyName("rol_Usuario")]
        public string Rol_Usuario { get; set; } = null!;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

}
