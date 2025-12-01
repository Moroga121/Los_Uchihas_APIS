using MAT03_Expedientes.Entities;
using MAT03_Expedientes.Repository;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MAT03_Expedientes.Services
{
    public class Expediente_EstudianteService : IExpediente_EstudianteService
    {
        private readonly Expediente_EstudiantesRepository _expediente_EstudiantesRepository;
        private readonly HttpClient _httpClient;


        public Expediente_EstudianteService(Expediente_EstudiantesRepository expediente_EstudiantesRepository, HttpClient httpClient)
        {
            _expediente_EstudiantesRepository = expediente_EstudiantesRepository;
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:9000/");
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

        public async Task<IResult> CRUDExpediente(Expediente_Estudiantes expediente_Estudiantes)
        {

            // validaciones de datos
            if (expediente_Estudiantes.Accion != "Eliminar")
            {
                var validacion = ValidarDatos(expediente_Estudiantes);
                if (validacion != null)
                {
                    return validacion;
                }
            }
            var (creada, mensajeSP) = await _expediente_EstudiantesRepository.CRUDExpediente(expediente_Estudiantes);

            await RegistrarBitacoraAsync(
                usuario: "usuario_actual",
                accion: expediente_Estudiantes.Accion,
                descripcion: JsonSerializer.Serialize(expediente_Estudiantes)
            );
            var data = creada ?? expediente_Estudiantes;

            if (mensajeSP.Contains("creado exitosamente"))
            {

                return Results.Created($"/expediente/{expediente_Estudiantes.numero_identificacion}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }

            if (mensajeSP.Contains("actualizado exitosamente"))
            {
                return Results.Created($"/expediente/{expediente_Estudiantes.numero_identificacion}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }
            if (mensajeSP.Contains("eliminado exitosamente"))
            {
                return Results.Ok(new { mensaje = mensajeSP });
            }
            else
            {
                return Results.BadRequest(new { mensaje = mensajeSP });
            }

        }
        public async Task<IEnumerable<Expediente_Estudiantes>> Obtener_Todos_Expedientes()
        {
            return await _expediente_EstudiantesRepository.Obtener_Todos_Expedientes();
        }
        public async Task<(Expediente_Estudiantes expediente_Estudiantes, string mensaje)> Obtener_Expediente_Por_ID(string numero_identificacion)
        {
            return await _expediente_EstudiantesRepository.Obtener_Expediente_Por_ID(numero_identificacion);
        }
        public IResult? ValidarDatos(Expediente_Estudiantes expediente_Estudiantes, string dominioPermitido = "cuc.cr")
        {
            if (string.IsNullOrEmpty(expediente_Estudiantes.numero_identificacion))
            {

                return Results.BadRequest(new { mensaje = "El número de identificación es obligatorio." });

            }
                
            if (string.IsNullOrEmpty(expediente_Estudiantes.tipo_identificacion))
            {
                return Results.BadRequest(new { mensaje = "El tipo de identificación es obligatorio." });
            }
            if (string.IsNullOrEmpty(expediente_Estudiantes.email))
            {
                return Results.BadRequest(new { mensaje = "El email es obligatorio." });
            }
                
            if (string.IsNullOrEmpty(expediente_Estudiantes.nombre))
            {

                return Results.BadRequest(new { mensaje = "El nombre es obligatorio." });

            }


            if (expediente_Estudiantes.fecha_nacimiento == null)
            {
                return Results.BadRequest(new { mensaje = "La fecha de nacimiento es obligatoria." });
            }
                
            if (expediente_Estudiantes.id_distrito == null || expediente_Estudiantes.id_distrito <= 0)
            {
                return Results.BadRequest(new { mensaje = "Debe seleccionar un distrito válido." });
            }
                
            if (string.IsNullOrEmpty(expediente_Estudiantes.otras_senas))
            {
                return Results.BadRequest(new { mensaje = "Las otras señas son obligatorias." });

            }
                
            if (string.IsNullOrEmpty(expediente_Estudiantes.telefono))
            {

                return Results.BadRequest(new { mensaje = "El teléfono es obligatorio." });

            }

            if (!string.IsNullOrEmpty(expediente_Estudiantes.numero_identificacion) && expediente_Estudiantes.numero_identificacion.Length > 22)
            {

                return Results.BadRequest(new { mensaje = "El número de identificación no puede exceder 22 caracteres." });

            }

            if (!string.IsNullOrEmpty(expediente_Estudiantes.tipo_identificacion) && expediente_Estudiantes.tipo_identificacion.Length > 20)
            {

                return Results.BadRequest(new { mensaje = "El tipo de identificación no puede exceder 20 caracteres." });

            }
                
            if (!string.IsNullOrEmpty(expediente_Estudiantes.email) && expediente_Estudiantes.email.Length > 100)
            {

                return Results.BadRequest(new { mensaje = "El email no puede exceder 100 caracteres." });

            }
                
            if (!string.IsNullOrEmpty(expediente_Estudiantes.nombre) && expediente_Estudiantes.nombre.Length > 100)
            {

                return Results.BadRequest(new { mensaje = "El nombre no puede exceder 100 caracteres." });

            }
                
            
            if (!string.IsNullOrEmpty(expediente_Estudiantes.otras_senas) && expediente_Estudiantes.otras_senas.Length > 255)
            {

                return Results.BadRequest(new { mensaje = "Las otras señas no pueden exceder 255 caracteres." });

            }
                
            if (!string.IsNullOrEmpty(expediente_Estudiantes.telefono) && expediente_Estudiantes.telefono.Length > 20)
            {

                return Results.BadRequest(new { mensaje = "El teléfono no puede exceder 20 caracteres." });

            }
                

            var formatoEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!string.IsNullOrEmpty(expediente_Estudiantes.email) && !System.Text.RegularExpressions.Regex.IsMatch(expediente_Estudiantes.email, formatoEmail))
            {

                return Results.BadRequest(new { mensaje = "El formato del email no es válido." });

            }
                

            if (!string.IsNullOrEmpty(expediente_Estudiantes.email) && !expediente_Estudiantes.email.EndsWith("@" + dominioPermitido, StringComparison.OrdinalIgnoreCase))
            {

                return Results.BadRequest(new { mensaje = $"El email debe pertenecer al dominio {dominioPermitido}." });

            }


            var soloLetrasYEspacios = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$";
            if (!string.IsNullOrEmpty(expediente_Estudiantes.nombre) && !System.Text.RegularExpressions.Regex.IsMatch(expediente_Estudiantes.nombre, soloLetrasYEspacios))
            {

                return Results.BadRequest(new { mensaje = "El nombre completo solo puede contener letras y espacios." });

            }
                
            

            if (expediente_Estudiantes.fecha_nacimiento != null && expediente_Estudiantes.fecha_nacimiento > DateTime.Now)
            {

                return Results.BadRequest(new { mensaje = "La fecha de nacimiento no puede ser futura." });


            }

            if (!string.IsNullOrEmpty(expediente_Estudiantes.telefono) && expediente_Estudiantes.telefono.Length < 8)
            {

                return Results.BadRequest(new { mensaje = "El teléfono debe tener al menos 8 dígitos." });

            }
               
            return null;
        }
        #region Bitacora

        public async Task RegistrarBitacoraAsync(string usuario, string accion, object descripcion)
        {
            var bitacora = new
            {
                Usuario = usuario,
                Accion = accion,
                Descripcion = descripcion
            };

            string json = JsonSerializer.Serialize(bitacora);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Se usa la ruta relativa, se envía a BaseAddress + ruta
                var response = await _httpClient.PostAsync("bitacora/registrar", content);

                if (!response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al registrar bitácora. StatusCode: {response.StatusCode}, Response: {apiResponse}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar con la API de bitácora: " + ex.Message);
            }
        }

        #endregion
    }
}
