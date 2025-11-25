using System.Text.Json.Serialization;

namespace Módulo_de_Oferta_académica_ACD5.Entities
{
    public class Periodo
    {
        public string ID_Periodo { get; set; } = null!;
        public int? Año { get; set; }
        public int? Numero_Periodo { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public string? Estado { get; set; }

        [JsonIgnore]
        public string? Mensaje { get; set; }

        [JsonIgnore]
        public string? Accion { get; set; }
    }
}
