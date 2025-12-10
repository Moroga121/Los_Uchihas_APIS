using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using USR3_Parametrización.Entities;
using USR3_Parametrización.Repository;

namespace USR3_Parametrización.Services
{
    public class ParametrizacionService : IParametrizacionService
    {
        private readonly ParametrizacionRepository _parametrizacionRepository;
        private readonly HttpClient _httpClient = new HttpClient();
        public ParametrizacionService(ParametrizacionRepository parametrizacionRepository, HttpClient httpClient)
        {
            _parametrizacionRepository = parametrizacionRepository;
            _httpClient = httpClient;
        }


        #region "Registrar Bitácora"


        public async Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/GEN01Bitacora/bitacora/registrar");

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

        #region "Obrtener Parametro por ID"

        public async Task<(Parametrizacion parametro, string mensaje)> Obtener_Parametro_Por_ID(string identificador_parametro)
        {
            var (parametro, mensaje) = await _parametrizacionRepository.Obtener_Parametro_Por_ID(identificador_parametro);
            return (parametro, mensaje);
        }

        #endregion

        #region "Obtener Todos Los Parametros"

        public async Task<IEnumerable<Parametrizacion>> Obtener_Todos_Los_Parametros()
        {


            var usuarios = await _parametrizacionRepository.Obtener_Todos_Los_Parametros();

            return usuarios;
        }

        #endregion

        #region "CRUD Usuarios"

        #region "Método Validaciones"

        public IResult? ValidarParametrizacion(Parametrizacion parametrizacion)
        {

            if (parametrizacion.Accion == "Insert" || parametrizacion.Accion == "Update")
            {

                
                if (string.IsNullOrWhiteSpace(parametrizacion.Identificador_Parametro))
                {
                    return Results.BadRequest(new { mensaje = "El identificador del parámetro no puede estar vacío." });
                }

                if (string.IsNullOrWhiteSpace(parametrizacion.Valor_Parametro))
                {
                    return Results.BadRequest(new { mensaje = "El valor del parámetro no puede estar vacío." });
                }

                
                if (parametrizacion.Identificador_Parametro.Length > 10)
                {
                    return Results.BadRequest(new { mensaje = "El identificador del parámetro no puede tener más de 10 caracteres." });
                }

                
                if (!Regex.IsMatch(parametrizacion.Identificador_Parametro, @"^[A-Z]+$"))
                {
                    return Results.BadRequest(new { mensaje = "El identificador del parámetro solo puede contener letras mayúsculas." });
                }

                
                if (parametrizacion.Valor_Parametro.Length > 500)
                {
                    return Results.BadRequest(new { mensaje = "El valor del parámetro no puede superar los 500 caracteres." });
                }

            }
            return null;

        }

        #endregion



        public async Task<IResult> CRUD_ParametrizacionAsync(Parametrizacion parametrizacion)
        {

            // LLamamos a la Validación


            if (parametrizacion.Accion != "Delete")
            {
                var validacion = ValidarParametrizacion(parametrizacion);
                if (validacion != null)
                {
                    return validacion;
                }
            }

            
            if (parametrizacion.Accion == "Delete")
            {
                
                var (parametroExistente, mensajeBusq) = await _parametrizacionRepository.Obtener_Parametro_Por_ID(parametrizacion.Identificador_Parametro);

                if (parametroExistente == null)
                {
                    return Results.NotFound(new { mensaje = "No se encontró el parámetro a eliminar." });
                }

               
                var mensajeElim = await _parametrizacionRepository.CRUD_ParametrizacionAsync(parametrizacion);

                if (mensajeElim.Contains("Parametro eliminado correctamente"))
                {
                    return Results.Ok(new
                    {
                        mensaje = mensajeElim,
                        parametro = new
                        {
                            parametroExistente.Identificador_Parametro,
                            parametroExistente.Valor_Parametro
                        }
                    });
                }

                if (mensajeElim.Contains("no existe"))
                {
                    return Results.NotFound(new { mensaje = mensajeElim });
                }

                return Results.BadRequest(new { mensaje = mensajeElim });
            }

            
            var mensajeSP = await _parametrizacionRepository.CRUD_ParametrizacionAsync(parametrizacion);

            
            if (mensajeSP.Contains("Parametro creado correctamente"))
            {
                return Results.Created($"/parametro/{parametrizacion.Identificador_Parametro}", new
                {
                    mensaje = mensajeSP,
                    parametro = new
                    {
                        parametrizacion.Identificador_Parametro,
                        parametrizacion.Valor_Parametro
                    }
                });
            }

            
            if (mensajeSP.Contains("Parametro actualizado correctamente"))
            {
                return Results.Ok(new
                {
                    mensaje = mensajeSP,
                    parametro = new
                    {
                        parametrizacion.Identificador_Parametro,
                        parametrizacion.Valor_Parametro
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

    }
}
