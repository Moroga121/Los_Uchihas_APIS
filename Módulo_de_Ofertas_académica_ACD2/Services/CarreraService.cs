using Módulo_de_Ofertas_académica_ACD2.Entities;
using Módulo_de_Ofertas_académica_ACD2.Repository;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Módulo_de_Ofertas_académica_ACD2.Services
{
    public class CarreraService : ICarreraService
    {
        private readonly CarreraRepository _carreraRepository;
        private readonly HttpClient _httpClient;

        public CarreraService(CarreraRepository carreraRepository, HttpClient httpClient)
        {
            _carreraRepository = carreraRepository;
            _httpClient = httpClient;
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

        public async Task<IEnumerable<Carrera>> Obtener_Todas_Las_Carreras()
            => await _carreraRepository.Obtener_Todas_Las_Carreras();

        public async Task<(Carrera carrera, string mensaje)> Obtener_Carrera_Por_ID(string idCarrera)
            => await _carreraRepository.Obtener_Carrera_Por_ID(idCarrera);

        public async Task<IEnumerable<Carrera>> Obtener_Carreras_Por_Institucion(string idInstitucion)
            => await _carreraRepository.Obtener_Carreras_Por_Institucion(idInstitucion);

        private IResult? ValidarCarrera(Carrera carrera)
        {
            if (carrera == null)
                return Results.BadRequest(new { mensaje = "Los datos de la carrera son requeridos." });

            if (string.IsNullOrWhiteSpace(carrera.ID_Carrera))
                return Results.BadRequest(new { mensaje = "El identificador de la carrera no puede estar vacío." });

            if (string.IsNullOrWhiteSpace(carrera.Accion))
                return Results.BadRequest(new { mensaje = "Debe indicar la acción a realizar (I, U o D)." });

            carrera.Accion = carrera.Accion.ToUpper();

            if (carrera.Accion == "D")
                return null;

            if (string.IsNullOrWhiteSpace(carrera.Nombre))
                return Results.BadRequest(new { mensaje = "El nombre de la carrera no puede estar vacío." });

            if (!Regex.IsMatch(carrera.Nombre, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$"))
                return Results.BadRequest(new { mensaje = "El nombre solo puede contener letras y espacios." });

            if (string.IsNullOrWhiteSpace(carrera.ID_Institucion))
                    return Results.BadRequest(new { mensaje = "El identificador de la institución no puede estar vacío." });

                if (string.IsNullOrWhiteSpace(carrera.ID_Director))
                    return Results.BadRequest(new { mensaje = "El identificador del director no puede estar vacío." });
            

            return null;
        }


        public async Task<IResult> CRUD_CarrerasAsync(Carrera carrera)
        {
            var validacion = ValidarCarrera(carrera);
            if (validacion != null) return validacion;

            if (carrera.Accion != "D" && string.IsNullOrWhiteSpace(carrera.ID_Director))
                return Results.BadRequest(new { mensaje = "Debe especificar el ID del director." });

            var mensajeSP = await _carreraRepository.CRUD_CarrerasAsync(carrera);

            if (mensajeSP.Contains("registrada correctamente"))
                return Results.Created($"/api/carrera/{carrera.ID_Carrera}", new 
                { 
                    mensaje = mensajeSP,
                    carrera = new 
                    {
                        carrera.ID_Carrera,
                        carrera.Nombre,
                        carrera.ID_Institucion,
                        carrera.ID_Director
                    }
                });

            if (mensajeSP.Contains("actualizada correctamente") || mensajeSP.Contains("eliminada correctamente"))
                return Results.Ok(new 
                { 
                    mensaje = mensajeSP,
                    carrera = new
                    {
                        carrera.ID_Carrera,
                        carrera.Nombre,
                        carrera.ID_Institucion,
                        carrera.ID_Director
                    }
                });

            if (mensajeSP.Contains("No se encontró"))
                return Results.NotFound(new { mensaje = mensajeSP });

            if (mensajeSP.Contains("Ya existe"))
                return Results.Conflict(new { mensaje = mensajeSP });

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

    }
}
