using System.Text.Json.Serialization;

namespace Módulo_de_Oferta_académica_ACD3.Entities
{
    public class Curso
    {
        public string ID_Curso { get; set; } = null!;
        public string ID_Carrera { get; set; } = null!;
        public int Nivel { get; set; }
        public string Nombre { get; set; } = null!;

        [JsonIgnore]
        public string? Mensaje { get; set; }
        [JsonIgnore]
        public string? Accion { get; set; }
    }
}
