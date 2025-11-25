using System.Text.Json.Serialization;

namespace Módulo_de_Oferta_académica_ACD4.Entities
{
    public class Grupo
    {
        public string ID_Grupo { get; set; } = null!;
        public int? Numero_Grupo { get; set; }
        public string ID_Curso { get; set; } = null!;
        public string ID_Profesor { get; set; } = null!;
        public string Horario { get; set; } = null!;
        public string ID_Periodo { get; set; } = null!;

        [JsonIgnore]
        public string? Mensaje { get; set; }
        [JsonIgnore]
        public string? Accion { get; set; }
    }
}
