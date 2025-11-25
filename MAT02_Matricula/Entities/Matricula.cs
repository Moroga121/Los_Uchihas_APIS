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
}
