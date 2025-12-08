using MAT01_Prematricula.Entities;
using MAT01_Prematricula.Repository;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MAT01_Prematricula.Services
{
    public class PrematriculaService : IPrematriculaService
    {

        private readonly PrematriculaRepository _prematriculaRepository;
        private readonly HttpClient _httpClient;

        public PrematriculaService(PrematriculaRepository prematriculaRepository, HttpClient httpClient)
        {
            _prematriculaRepository = prematriculaRepository;
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("https://tiusr21pl.cuc-carrera-ti.ac.cr/GEN01Bitacora/bitacora/");
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
        public async Task<IResult> CRUDPrematricula(Prematricula prematricula)
        {
            // validaciones
            if (prematricula.Accion != "Eliminar")
            {
                var validacion = ValidarDatos(prematricula);
                if (validacion != null)
                    return validacion;
            }

            var (creada, mensajeSP) = await _prematriculaRepository.CRUDPrematricula(prematricula);


            var data = creada ?? prematricula;

            if (mensajeSP.Contains("Prematrícula creada exitosamente"))
            {

                return Results.Created($"/Prematricula/{data.id_prematricula}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }

            if (mensajeSP.Contains("Prematrícula actualizada exitosamente"))
            {
                return Results.Created($"/Prematricula/{data.id_prematricula}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }
            if (mensajeSP.Contains("Prematrícula eliminada exitosamente"))
            {
                return Results.Ok(new { mensaje = mensajeSP });
            }

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        public async Task<IEnumerable<Prematricula>> Obtener_Todas_Prematriculas()
        {
            var prematriculas = await _prematriculaRepository.Obtener_Todas_Prematriculas();

            return prematriculas;
        }

        public async Task<IEnumerable<Prematricula>> Obtener_Prematricula_Por_Identificacion(string numero_identificacion)
        {
            var prematriculas = await _prematriculaRepository.Obtener_Prematricula_Por_Identificacion(numero_identificacion);
            return prematriculas;
        }


        public async Task<(Prematricula prematricula, string mensaje)> Obtener_Prematricula_Por_ID(string Id_Prematricula)
        {
            var prematriculas = await _prematriculaRepository.Obtener_Prematricula_Por_ID(Id_Prematricula);

            return prematriculas;
        }
        public IResult? ValidarDatos(Prematricula prematricula)
        {
            if (string.IsNullOrEmpty(prematricula.numero_identificacion))
                return Results.BadRequest("El número de identificación es obligatorio.");
            if (string.IsNullOrEmpty(prematricula.carrera))
                return Results.BadRequest("La carrera es obligatoria.");
            if (string.IsNullOrEmpty(prematricula.curso))
                return Results.BadRequest("El curso es obligatorio.");
            if (string.IsNullOrEmpty(prematricula.Id_Periodo))
                return Results.BadRequest("El ID del período es obligatorio.");
            if (!string.IsNullOrEmpty(prematricula.numero_identificacion) && prematricula.numero_identificacion.Length > 22)
                return Results.BadRequest("El número de identificación no puede exceder 22 caracteres.");
            if (!string.IsNullOrEmpty(prematricula.carrera) && prematricula.carrera.Length > 100)
                return Results.BadRequest("El nombre de la carrera no puede exceder 100 caracteres.");
            if (!string.IsNullOrEmpty(prematricula.curso) && prematricula.curso.Length > 50)
                return Results.BadRequest("El nombre del curso no puede exceder 50 caracteres.");
            if (!string.IsNullOrEmpty(prematricula.observaciones) && prematricula.observaciones.Length > 255)
                return Results.BadRequest("Las observaciones no pueden exceder 255 caracteres.");

            return null;

        }

}
}
