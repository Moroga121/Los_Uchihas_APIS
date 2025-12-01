using Módulo_de_Oferta_académica_ACD5.Entities;
using Módulo_de_Oferta_académica_ACD5.Repository;
using System.Text.Json;

namespace Módulo_de_Oferta_académica_ACD5.Service
{
    public class PeriodoService : IPeriodoService
    {
        private readonly PeriodoRepository _periodoRepository;
        private readonly HttpClient _httpClient;

        public PeriodoService(PeriodoRepository periodoRepository, HttpClient httpClient)
        {
            _periodoRepository = periodoRepository;
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

        private static IResult? ValidarPeriodo(Periodo periodo)
        {
            if (periodo == null)
                return Results.BadRequest(new { mensaje = "Los datos del periodo son requeridos." });

            if (string.IsNullOrWhiteSpace(periodo.ID_Periodo))
                return Results.BadRequest(new { mensaje = "El identificador del periodo no puede estar vacío." });

            if (string.IsNullOrWhiteSpace(periodo.Accion))
                return Results.BadRequest(new { mensaje = "Debe indicar la acción (I, U, D)." });

            periodo.Accion = periodo.Accion.ToUpper();

            if (periodo.Accion != "D")
            {
                if (!periodo.Año.HasValue || periodo.Año <= 0)
                    return Results.BadRequest(new { mensaje = "El año del periodo es requerido y debe ser mayor que 2000." });

                if (!periodo.Numero_Periodo.HasValue || periodo.Numero_Periodo <= 0)
                    return Results.BadRequest(new { mensaje = "El número de periodo es requerido y debe ser mayor que 0." });

                if (periodo.Fecha_Inicio == default)
                    return Results.BadRequest(new { mensaje = "Debe indicar la fecha de inicio." });

                if (periodo.Fecha_Fin == default)
                    return Results.BadRequest(new { mensaje = "Debe indicar la fecha de fin." });

                if (periodo.Fecha_Fin <= periodo.Fecha_Inicio)
                    return Results.BadRequest(new { mensaje = "La fecha de fin debe ser posterior a la fecha de inicio." });
            }

            return null;
        }

        private static void CalcularEstado(Periodo periodo)
        {
            var hoy = DateTime.Now.Date;
            if (periodo.Fecha_Inicio.Date > hoy)
                periodo.Estado = "Futuro";
            else if (periodo.Fecha_Fin.Date < hoy)
                periodo.Estado = "Cerrado";
            else
                periodo.Estado = "Activo";
        }

        public async Task<IResult> CRUD_PeriodosAsync(Periodo periodo)
        {
            var validacion = ValidarPeriodo(periodo);
            if (validacion != null)
                return validacion;

            var mensajeSP = await _periodoRepository.CRUD_PeriodosAsync(periodo);

            CalcularEstado(periodo);

            if (mensajeSP.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                return Results.Ok(new
                {
                    mensaje = mensajeSP,
                    periodo = new
                    {
                        periodo.ID_Periodo,
                        periodo.Año,
                        periodo.Numero_Periodo,
                        periodo.Fecha_Inicio,
                        periodo.Fecha_Fin,
                        periodo.Estado
                    }
                });

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        public async Task<IEnumerable<Periodo>> Obtener_Todos_Los_Periodos()
        {
            return await _periodoRepository.Obtener_Todos_Los_Periodos();
        }

        public async Task<(Periodo periodo, string mensaje)> Obtener_Periodo_Por_ID(string idPeriodo)
        {
            return await _periodoRepository.Obtener_Periodo_Por_ID(idPeriodo);
        }

    }
}
