using System.Net.Http;
using System.Text.Json;
using ACA1_Promedio.Entities;
using ACA1_Promedio.Repository;

namespace ACA1_Promedio.Services
{
    public class HistorialService : IHistorialService
    {
        private readonly NotaRepository _notaRepository;
        private readonly HttpClient _httpClient = new HttpClient();
        public HistorialService(NotaRepository notaRepository, HttpClient httpClient)
        {
            _notaRepository = notaRepository;
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

        public async Task<IEnumerable<HistorialAcademico>> ObtenerHistorialAsync(string tipoIdentificacion,string numeroIdentificacion, int? año, string? periodo)
        {
            if (string.IsNullOrWhiteSpace(tipoIdentificacion))
                throw new ArgumentException("El Tipo de Identificación es obligatorio.");

            if (string.IsNullOrWhiteSpace(numeroIdentificacion))
                throw new ArgumentException("La Identificación es obligatoria.");

            var notas = await _notaRepository.ObtenerNotasPorEstudianteAsync(tipoIdentificacion, numeroIdentificacion, año, periodo);

            var historial = notas
                .GroupBy(n => n.Curso)
                .Select(g => new HistorialAcademico
                {
                    CodigoCurso = g.Key.Substring(0, 3).ToUpper(),
                    NombreCurso = g.Key,
                    Promedio = Math.Round( g.Average(x => x.NotaObtenida), 2)
                })
                .ToList();

            return historial;
        }

    }
}
