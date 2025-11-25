using System.Text.Json.Serialization;

namespace Módulo_de_Ofertas_académica_ACD2.Entities
{
    public class Carrera
    {
        public string? ID_Carrera { get; set; } = null!;
        public string? Nombre { get; set; } = null!;
        public string? ID_Institucion { get; set; } = null!;
        public string? ID_Director { get; set; } = null!;

        [JsonIgnore]
        public string? Mensaje { get; set; }
        [JsonIgnore]
        public string? Accion { get; set; }
    }
}
