using System.Text.Json.Serialization;

namespace Módulo_de_Oferta_académica_ACD1.Entities
{
    public class Institucion
    {
        public string ID_Institucion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        [JsonIgnore]
        public string? Mensaje { get; set; }
        [JsonIgnore]
        public string? Accion { get; set; }
    }
}
