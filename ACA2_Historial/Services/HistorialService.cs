using System.Net.Http;
using System.Text.Json;
using ACA2_Historial.Entities;
using ACA2_Historial.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACA2_Historial.Services
{
    public class HistorialService : IHistorialService
    {

        private readonly HistorialRepository _historialRepository;
        private readonly HttpClient _httpClient = new HttpClient();
        public HistorialService(HistorialRepository historialRepository, HttpClient httpClient)
        {
            _historialRepository = historialRepository;
            _httpClient = httpClient;
        }

        public async Task<(bool Exito, string Mensaje, IEnumerable<Estudiante_Matriculado>? Estudiante_Matriculado)> ObtenerEstudiantesMatriculaPeriodoAsync(string Periodo)
        {
            if (string.IsNullOrWhiteSpace(Periodo))
                return (false, "El periodo debe ser nulo.", null);

            try
            {
                var lista = await _historialRepository.ObtenerEstudiantesMatriculaPeriodoAsync(Periodo);

                if (lista is null)
                    return (false, $"No se encontró datos relacionados con el periodo número {Periodo}.", null);

                return (true, "", lista);
            }
            catch (Exception ex)
            {
                return (false, $"Error al consultar el periodo: {ex.Message}", null);
            }
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

        public async Task<IEnumerable<MatriculaDto>> ObtenerMatriculasAsync()
        {
            var matriculas = await _historialRepository.ObtenerTodasMatriculasAsync();

            if (matriculas == null || !matriculas.Any())
                return Enumerable.Empty<MatriculaDto>();

            return matriculas;
        }

    }
}