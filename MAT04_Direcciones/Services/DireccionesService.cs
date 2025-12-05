using MAT04_Direcciones.Entities;
using MAT04_Direcciones.Repository;
using MySqlX.XDevAPI.Common;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MAT04_Direcciones.Services
{
    public class DireccionesService : IDireccionesService
    {
        private readonly DireccionesRepository _direccionesRepository;
        private readonly HttpClient _httpClient;

        public DireccionesService(DireccionesRepository direccionesRepository, HttpClient httpClient)
        {
            _direccionesRepository = direccionesRepository;
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("https://tiusr21pl.cuc-carrera-ti.ac.cr/GEN01Bitacora/");
        }
        public async Task<IEnumerable<Entities.Provincias>> Obtener_Todos_Provincias()
        {
            return await _direccionesRepository.Obtener_Todos_Provincias();
        }
        public async Task<(IEnumerable<cantones> cantones, string mensaje)> Obtener_Cantones_Por_Provincia(string provincia)
        {
            if (string.IsNullOrWhiteSpace(provincia))
                return (Enumerable.Empty<cantones>(), "El nombre de la provincia es obligatorio.");

            if (provincia.Length > 50)
                return (Enumerable.Empty<cantones>(), "El nombre no puede superar los 50 caracteres.");

            var (cantones, mensajeSP) = await _direccionesRepository.Obtener_Cantones_Por_Provincia(provincia);
            return (cantones, mensajeSP);
        }
        public async Task<(IEnumerable<distritos> distritos, string mensaje)> Obtener_Distritos_Por_Canton_Provincia(string provincia , string canton)
        {
            if (string.IsNullOrWhiteSpace(provincia))
                return (Enumerable.Empty<distritos>(), "El nombre de la provincia es obligatorio.");

            if (provincia.Length > 50)
                return (Enumerable.Empty<distritos>(), "Los datos ingresados no puede superar los 50 caracteres.");

            if(string.IsNullOrWhiteSpace(canton))
                return (Enumerable.Empty<distritos>(), "El nombre del canton es obligatorio.");

            if (canton.Length > 50)
                return (Enumerable.Empty<distritos>(), "Los datos ingresados no puede superar los 50 caracteres.");

            var (distritos, mensajeSP) = await _direccionesRepository.Obtener_Distritos_Por_Canton_Provincia(provincia, canton);

            return (distritos, mensajeSP);
        }

        public async Task<IEnumerable<Direcciones>> Obtener_Direccion_Expediente(int id_distrito)
        {
            return await _direccionesRepository.Obtener_Direccion_Expediente(id_distrito);
        }


        #region Bitacora

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
    }
}
