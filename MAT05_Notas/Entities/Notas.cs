using System.Text.Json.Serialization;

namespace MAT05_Notas.Entities
{
    public class Notas
    {

        public int id_nota { get; set; }
        public string? numero_identificacion { get; set; }
        public string? id_rubro { get; set; }
        public decimal valor { get; set; }

        [JsonIgnore]
        public string? Accion { get; set; }
    }
}
