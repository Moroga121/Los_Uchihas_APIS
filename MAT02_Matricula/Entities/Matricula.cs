using System.Text.Json.Serialization;

namespace MAT02_Matricula.Entities
{
    public class Matricula
    {

        public int? Id_matricula { get; set; }
        public string? numero_identificacion { get; set; }
        public string? curso { get; set; }
        public string? grupo { get; set; }
        public string? Id_periodo { get; set; }
        [JsonIgnore]
        public string? Accion { get; set; } = null!;
    }


    public class Prematricula
    {
        public int id_prematricula { get; set; }
        public string? numero_identificacion { get; set; }
        public string? carrera { get; set; }
        public string? curso { get; set; }
        public string? observaciones { get; set; }
        public string? Id_Periodo { get; set; }
        [JsonIgnore]
        public string Accion { get; set; } = null!;
    }

    public class Usuario
    {
        public string Identificacion { get; set; } = null!;
        public string Tipo_Identificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Contrasena { get; set; } = null!;

        [JsonPropertyName("rol_Usuario")]
        public string Rol_Usuario { get; set; } = null!;
    }


    public class Curso
    {
        [JsonPropertyName("iD_Curso")]
        public string ID_Curso { get; set; } = null!;

        [JsonPropertyName("iD_Carrera")]
        public string ID_Carrera { get; set; } = null!;

        [JsonPropertyName("nivel")]
        public int Nivel { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = null!;

    }

    public class Carrera
    {
        public string iD_Carrera { get; set; } = null!;
        public string nombre { get; set; } = null!;
        public string iD_Institucion { get; set; } = null!;
        public string iD_Director { get; set; } = null!;
    }


    public class Periodo
    {
        [JsonPropertyName("iD_Periodo")]
        public string ID_Periodo { get; set; } = null!;

        [JsonPropertyName("año")]
        public int? Año { get; set; }

        [JsonPropertyName("numero_Periodo")]
        public int? Numero_Periodo { get; set; }

        [JsonPropertyName("fecha_Inicio")]
        public DateTime Fecha_Inicio { get; set; }

        [JsonPropertyName("fecha_Fin")]
        public DateTime Fecha_Fin { get; set; }

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }


    }

    public class Grupo
    {
        [JsonPropertyName("iD_Grupo")]
        public string ID_Grupo { get; set; } = null!;

        [JsonPropertyName("numero_Grupo")]
        public int? Numero_Grupo { get; set; }

        [JsonPropertyName("iD_Curso")]
        public string ID_Curso { get; set; } = null!;

        [JsonPropertyName("iD_Profesor")]
        public string ID_Profesor { get; set; } = null!;

        [JsonPropertyName("horario")]
        public string Horario { get; set; } = null!;

        [JsonPropertyName("iD_Periodo")]
        public string ID_Periodo { get; set; } = null!;


    }


}
