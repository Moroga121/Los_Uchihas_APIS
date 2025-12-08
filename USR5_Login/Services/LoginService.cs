using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using USR5_Login.Entities;
using USR5_Login.Repository;


namespace USR5_Login.Services
{
    public class LoginService : ILoginService
    {

        private readonly LoginRepository _loginRepository;
        private readonly TokenRepository _tokenRepository;
        private readonly HttpClient _httpClient;

        public LoginService(LoginRepository loginRepository, TokenRepository tokenRepository, HttpClient httpClient)
        {
            _loginRepository = loginRepository;
            _tokenRepository = tokenRepository;
            _httpClient = httpClient;
        }

        #region "Registrar Bitácora"


        public async Task<(bool, string mensaje)> RegistrarBitacoraAsync(string email,string accion, object descripcion,CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/bitacora/registrar");

         
            // Crear el JSON a enviar
            var body = new
            {
                Usuario = email,
                Accion = accion,
                Descripcion = descripcion
            };

            // Serializar a JSON
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request, ct);
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Bitácora registrada exitosamente");
                }
                else
                {
                    var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, string?>>(cancellationToken: ct);
                    if (payload != null && payload.TryGetValue("mensaje", out var m))
                    {
                        return (false, m ?? "Error desconocido al registrar bitácora");
                    }
                    return (false, "Error desconocido al registrar bitácora");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error al procesar la respuesta de la API: {ex.Message}");
            }
        }


        #endregion
        
        public async Task<IResult> ValidarUsuarioAsync(Login login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Contrasena))
            {
                return Results.BadRequest(new { mensaje = "Usuario y contraseña son requeridos." });
            }

            login.Contrasena = Encrypt(login.Contrasena);
            var resultado = await _loginRepository.ValidarUsuarioAsync(login);

            if (resultado.Mensaje == "Login Exitoso")
            {
                var token = _tokenRepository.GenerarToken(login.Email);

                var loginResult = Results.Created("/login", new
                {
                    expires_in = token.Expires_In,
                    access_token = token.Access_Token,
                    refresh_token = token.Refresh_Token,
                    usuarioID = login.Email,
                    Idetificacion = login.Identificacion
                });
                // Registrar intento exitoso en la bitacora del login
                await RegistrarBitacoraAsync(
                     email: login.Email,
                   accion: "Intento de logeo exitoso",
                   descripcion: loginResult
               );

                // Registrar acceso al sistema
                await RegistrarBitacoraAsync(
                     email: login.Email,
                   accion: "Acceso al sistema",
                   descripcion: loginResult
               );

                return loginResult;
            }
            // Registrar intento fallido en la bitacora del login
            await RegistrarBitacoraAsync(
                 email: login.Email,
               accion: "Intento de logeo fallido",
               descripcion: "Credenciales inválidas"
           );
            return Results.Json(new { mensaje = "Usuario y/o contraseña incorrectos" }, statusCode: 401);
        }


        #region "Métodos de Encriptación y Desincriptación"

        private static readonly string Key = "0123456789abcdef";
        private static readonly string IV = "abcdef0123456789";

        // Método  para Encriptar

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Padding = PaddingMode.PKCS7;

                using var msEncrypt = new MemoryStream();
                using (var csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                {
                    swEncrypt.Write(plainText);
                }

                byte[] encryptedBytes = msEncrypt.ToArray();
                string base64 = Convert.ToBase64String(encryptedBytes);

                return base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
            }
        }

        // Método para Desencriptar

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            string base64 = cipherText.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] cipherBytes = Convert.FromBase64String(base64);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Padding = PaddingMode.PKCS7;

                using var msDecrypt = new MemoryStream(cipherBytes);
                using var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8);

                return srDecrypt.ReadToEnd();
            }
        }

        #endregion

    }



}
