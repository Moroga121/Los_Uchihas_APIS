using Proyecto_PrograV.Entities;
using Proyecto_PrograV.Repository;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace Proyecto_PrograV.Services
{
    public class UsuarioService : IUsuarioService
    {

        private readonly UsuarioRepository _usuarioRepository;
        private readonly HttpClient _httpClient;

        public UsuarioService(UsuarioRepository usuarioRepository, HttpClient httpClient)
        {
            _usuarioRepository = usuarioRepository;
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
            {

                _httpClient.BaseAddress = new Uri("http://localhost:9000/");

            }

        }


        #region "Registrar Bitácora"


        public async Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/bitacora/registrar");

            // Agregar token al header
            request.Headers.Add("access_token", accessToken);

            // Crear el JSON a enviar
            var body = new
            {
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

        #region "Obtener Tipos de Identificación"

        public async Task<IEnumerable<Tipos_Identificacion>> Obtener_Tipos_De_Identificacion()
        {

            var tipos_ident = await _usuarioRepository.Obtener_Tipos_De_Identificacion();


            



            return tipos_ident;


        }



        #endregion

        #region "Obtener Dominios"

        public async Task<IEnumerable<Usuario>> Obtener_Dominios()
        {
            var dominios = await _usuarioRepository.Obtener_Dominios();

            return dominios;


        }


        #endregion

        #region "Obtener Todos Los Usuarios"

        public async Task<IEnumerable<Usuario>> Obtener_Todos_Los_Usuarios()
        {

            var usuarios = await _usuarioRepository.Obtener_Todos_Los_Usuarios();


            foreach (var usuario in usuarios)
            {
                if (!string.IsNullOrEmpty(usuario.Contrasena))
                {

                    usuario.Contrasena = Decrypt(usuario.Contrasena);

                }

            }
            return usuarios;
        }

        #endregion

        #region "Obtener Usuario por ID"

        public async Task<(Usuario usuario, string mensaje)> Obtener_Usuario_Por_Identificacion(string identificacion)
        {
            var (usuario, mensaje) = await _usuarioRepository.Obtener_Usuario_Por_Identificacion(identificacion);


            if (usuario != null && !string.IsNullOrEmpty(usuario.Contrasena))
            {

                usuario.Contrasena = Decrypt(usuario.Contrasena);

            }



            return (usuario, mensaje);
        }

        public async Task<(Usuario usuario, string mensaje)> Obtener_Usuario_Por_ID(string identificacion)
        {
            var (usuario, mensaje) = await _usuarioRepository.Obtener_Usuario_Por_ID(identificacion);


            if (usuario != null && !string.IsNullOrEmpty(usuario.Contrasena))
            {

                usuario.Contrasena = Decrypt(usuario.Contrasena);

            }


            return (usuario, mensaje);
        }

        #endregion

        #region "Filtrar Usuario"

        public async Task<IResult> Obtener_Usuarios_FiltradosAsync(string identificacion, string nombre, string rol, string tipo, string dominio)
        {

            var (usuarios, mensajeSP) = await _usuarioRepository.Obtener_Usuarios_FiltradosAsync(identificacion, nombre, rol, tipo, dominio);


            if (!string.IsNullOrEmpty(mensajeSP))
            {
                if (mensajeSP.Contains("no encontrado"))
                {

                    return Results.NotFound(new { mensaje = mensajeSP });

                }


                if (mensajeSP.Contains("Rol no existente"))
                {
                    return Results.BadRequest(new { mensaje = mensajeSP });

                }

            }

            foreach (var usuario in usuarios)
            {
                if (!string.IsNullOrEmpty(usuario.Contrasena))
                {
                    usuario.Contrasena = Decrypt(usuario.Contrasena);
                }

            }


            return Results.Ok(usuarios);
        }

        #endregion

        #region "Cambiar Contraseña Usuario"


        public async Task<IResult> Cambiar_Contrasena_UsuarioAsync(Usuario usuario)
        {

            if (string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                return Results.BadRequest(new { mensaje = "Debe ingresar un correo y una nueva contraseña." });
            }

            
            usuario.Contrasena = Encrypt(usuario.Contrasena);

            

            var resultado = await _usuarioRepository.CambiarContraAsync(usuario);

            
            
            if (resultado.Mensaje.Contains("correctamente"))
            {
                return Results.Ok(new
                {
                    mensaje = resultado.Mensaje,
                    usuario = new
                    {
                        usuario.Email,
                        usuario.Contrasena

                    }
                });
            }

            if (resultado.Mensaje.Contains("no existe"))
            {
                return Results.NotFound(new { mensaje = resultado.Mensaje });
            }

            return Results.BadRequest(new { mensaje = resultado.Mensaje });
        }



        #endregion

        #region "CRUD Usuarios"

        #region "Método Validaciones"

        public IResult? ValidarUsuario(Usuario usuario)
        {

            if (usuario.Accion == "Insert" || usuario.Accion == "Update")
            {

                usuario.Nombre = usuario.Nombre.Trim();
                usuario.Email = usuario.Email.Trim();

                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                {
                    return Results.BadRequest(new { mensaje = "El nombre completo no puede estar vacío." });
                }



                if (string.IsNullOrWhiteSpace(usuario.Email))
                {
                    return Results.BadRequest(new { mensaje = "El email no puede estar vacío." });
                }



                var emailRegex = new Regex(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$");
                if (!emailRegex.IsMatch(usuario.Email))
                {
                    return Results.BadRequest(new { mensaje = "Formato de email inválido." });
                }



                var dominio = usuario.Email.Split('@').Last().ToLower();

                if (dominio == "cuc.cr")
                {
                    if (usuario.Rol_Usuario.ToLower() != "est")
                    {
                        return Results.BadRequest(new { mensaje = "Los emails de dominio cuc.cr deben tener rol Estudiante." });
                    }
                }
                else if (dominio == "cuc.ac.cr")
                {
                    if (usuario.Rol_Usuario.ToLower() != "prf" && usuario.Rol_Usuario.ToLower() != "adm")
                    {
                        return Results.BadRequest(new { mensaje = "Los emails de dominio cuc.ac.cr deben tener rol Profesor o Administrador." });
                    }
                }
                else
                {
                    return Results.BadRequest(new { mensaje = "El dominio del email debe ser cuc.cr o cuc.ac.cr." });
                }

            }
            return null;

        }

        #endregion



        public async Task<IResult> CRUD_UsuariosAsync(Usuario usuario)
        {

            // LLamamos a la Validación


            if (usuario.Accion != "Delete")
            {
                var validacion = ValidarUsuario(usuario);
                if (validacion != null)
                {
                    return validacion;
                }


                if (!string.IsNullOrEmpty(usuario.Contrasena) &&
                    (usuario.Accion == "Insert" || usuario.Accion == "Update"))
                {
                    usuario.Contrasena = Encrypt(usuario.Contrasena);
                }
            }


            if (usuario.Accion == "Delete")
            {

                var (usuarioExistente, mensajeBusq) = await _usuarioRepository.Obtener_Usuario_Por_Identificacion(usuario.Identificacion);

                if (usuarioExistente == null)
                {
                    return Results.NotFound(new { mensaje = "No se encontró el usuario a eliminar." });
                }


                var mensajeElim = await _usuarioRepository.CRUD_UsuariosAsync(usuario);


                if (mensajeElim.Contains("Usuario eliminado correctamente"))
                {
                    return Results.Ok(new
                    {
                        mensaje = mensajeElim,
                        usuario = new
                        {
                            usuarioExistente.Identificacion,
                            usuarioExistente.Tipo_Identificacion,
                            usuarioExistente.Nombre,
                            usuarioExistente.Email,
                            usuarioExistente.Contrasena,
                            usuarioExistente.Rol_Usuario
                        }
                    });
                }

                if (mensajeElim.Contains("no existe"))
                {
                    return Results.NotFound(new { mensaje = mensajeElim });
                }

                return Results.BadRequest(new { mensaje = mensajeElim });
            }


            var mensajeSP = await _usuarioRepository.CRUD_UsuariosAsync(usuario);


            if (mensajeSP.Contains("Usuario creado correctamente"))
            {
             

                return Results.Created($"/usuario/{usuario.Identificacion}", new
                {
                    mensaje = mensajeSP,
                    usuario = new
                    {
                        usuario.Identificacion,
                        usuario.Tipo_Identificacion,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Contrasena,
                        usuario.Rol_Usuario
                    }
                });
            }


            if (mensajeSP.Contains("Usuario actualizado correctamente"))
            {

                return Results.Ok(new
                {
                    mensaje = mensajeSP,
                    usuario = new
                    {
                        usuario.Identificacion,
                        usuario.Tipo_Identificacion,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Contrasena,
                        usuario.Rol_Usuario
                    }
                });
            }


            if (mensajeSP.Contains("no existe"))
            {
                return Results.NotFound(new { mensaje = mensajeSP });
            }

            if (mensajeSP.Contains("Ya existe"))
            {
                return Results.Conflict(new { mensaje = mensajeSP });
            }

            return Results.BadRequest(new { mensaje = mensajeSP });


        }

        #endregion

        #region "Método para Encriptar y Desencriptar"

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
