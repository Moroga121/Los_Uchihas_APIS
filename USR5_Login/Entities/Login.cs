using System.Text.Json.Serialization;

namespace USR5_Login.Entities
{
    public class Login
    {

        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;

        [JsonIgnore]
        public string Mensaje { get; set; } = null!;
    }
}
