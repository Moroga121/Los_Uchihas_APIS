using System.Text.Json.Serialization;

namespace MAT04_Direcciones.Entities
{
    public class Provincias
    {
        public int id_provincia { get; set; }
        public string? nombre { get; set; }

        [JsonIgnore]
        public string? Mensaje { get; set; }
    }
}
