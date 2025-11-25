using System.Text.Json.Serialization;

namespace USR3_Parametrización.Entities
{
    public class Parametrizacion
    {

        public string Identificador_Parametro { get; set; } = null!;

        public string Valor_Parametro { get; set; } = null!;

        [JsonIgnore]
        public string Mensaje { get; set; } = null!;

        [JsonIgnore]
        public string Accion { get; set; } = null!;


    }
}
