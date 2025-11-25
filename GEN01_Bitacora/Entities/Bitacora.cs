using System.Text.Json.Serialization;

namespace GEN01_Bitacora.Entities
{
    public class Bitacora
    {
        public string? Usuario { get; set; }   
        public string? Descripcion { get; set; }
        public string? Accion { get; set; }
        
        public DateTime Fecha { get; set; }

    }
}
