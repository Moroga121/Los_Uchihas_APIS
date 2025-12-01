using MAT05_Notas.Entities;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MAT05_Notas.Services
{
    public class NotasService : INotasService
    {
        private readonly Repository.NotasRepository _notasRepository;
        private readonly HttpClient _httpClient;

        public NotasService(Repository.NotasRepository notasRepository, HttpClient httpClient)
        {
            _notasRepository = notasRepository;
             _httpClient = httpClient;
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("https://tiusr21pl.cuc-carrera-ti.ac.cr/GEN01Bitacora/");
        }

        public async Task<IResult> Cargar_Desglose(DesgloseRubro desgloserubro)
        {
            // Validar suma de porcentajes ANTES de insertar
            var suma = desgloserubro.rubros.Sum(r => r.porcentaje);

            if (suma > 100)
            {
                return Results.BadRequest(new { mensaje = "Los porcentajes sobrepasan el 100%" });
            }

            if (suma < 100)
            {
                return Results.BadRequest(new { mensaje = "Los porcentajes NO suman 100%" });
            }

            var (mensaje, objetos) = await _notasRepository.Cargar_Desglose(desgloserubro);

            if (mensaje.Contains("Desglose completado correctamente.", StringComparison.OrdinalIgnoreCase))
            {
                if (objetos != null && objetos.Count > 0)
                    return Results.Created($"/cargardesglose/{objetos.First().rubros}", new { mensaje, data = objetos });

            }

            return Results.BadRequest(new { mensaje });
        }

        public async Task<IResult> Asignar_Actualizar_Nota(Notas notas)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(notas.numero_identificacion))
                return Results.BadRequest(new { mensaje = "El número de identificación es obligatorio." });
            if (notas.numero_identificacion.Length > 22)
                return Results.BadRequest(new { mensaje = "El número de identificación no puede superar los 22 caracteres." });

            if (string.IsNullOrWhiteSpace(notas.id_rubro))
                return Results.BadRequest(new { mensaje = "El id_rubro es obligatorio." });
            if (notas.id_rubro.Length > 10)
                return Results.BadRequest(new { mensaje = "El id_rubro no puede superar los 10 caracteres." });

            if (notas.valor < 0)
                return Results.BadRequest(new { mensaje = "El valor de la nota no puede ser negativo." });

            if (string.IsNullOrWhiteSpace(notas.Accion))
                return Results.BadRequest(new { mensaje = "La acción es obligatoria (Crear o Actualizar)." });
            if (notas.Accion != "Crear" && notas.Accion != "Actualizar")
                return Results.BadRequest(new { mensaje = "La acción debe ser 'Crear' o 'Actualizar'." });
            if (notas.valor < 1 || notas.valor >100)
                return Results.BadRequest(new { mensaje = "El valor debe estar en el rango de 1 a 100." });

            // Llamar al repositorio
            var (creada, mensajeSP) = await _notasRepository.Asignar_Actualizar_Nota(notas);

            var data = creada ?? notas;

            if (mensajeSP.Contains("Nota asignada correctamente.", StringComparison.OrdinalIgnoreCase))
                return Results.Created($"/notas/{notas.id_nota}", new
                {
                    mensaje = mensajeSP,
                    data
                });

            if (mensajeSP.Contains("Nota actualizada correctamente.", StringComparison.OrdinalIgnoreCase))
                return Results.Created($"/notas/{notas.id_nota}", new
                {
                    mensaje = mensajeSP,
                    data
                });

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        public async Task<IResult> Obtener_Desglose_Por_ID(string grupo, string curso)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(grupo))
                    return Results.BadRequest(new { mensaje = "El grupo es obligatorio." });
                if (grupo.Length > 20)
                    return Results.BadRequest(new { mensaje = "El grupo no puede superar los 20 caracteres." });

                if (string.IsNullOrWhiteSpace(curso))
                    return Results.BadRequest(new { mensaje = "El curso es obligatorio." });
                if (curso.Length > 50)
                    return Results.BadRequest(new { mensaje = "El curso no puede superar los 50 caracteres." });

                var (desgloserubro, mensaje) = await _notasRepository.Obtener_Desglose_Por_ID(grupo, curso);

                if (mensaje == "Desglose encontrado correctamente.")
                    return Results.Ok(new { mensaje, data = desgloserubro });

                return Results.BadRequest(new { mensaje });
            }
            catch (Exception ex)
            {
                return Results.NotFound(new { mensaje = ex.Message });
            }
        }

        public async Task<IResult> Obtener_Notas_By_Id(string numero_identificacion, string curso)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numero_identificacion))
                return Results.BadRequest(new { mensaje = "El número de identificación es obligatorio." });
            if (numero_identificacion.Length > 22)
                return Results.BadRequest(new { mensaje = "El número de identificación no puede superar los 22 caracteres." });

            if (string.IsNullOrWhiteSpace(curso))
                return Results.BadRequest(new { mensaje = "El curso es obligatorio." });
            if (curso.Length > 50)
                return Results.BadRequest(new { mensaje = "El nombre del curso no puede superar los 50 caracteres." });

            var (notas, mensaje) = await _notasRepository.Obtener_Notas_By_Id(numero_identificacion, curso);

            if (mensaje == "Notas obtenidas correctamente para el estudiante.")
                return Results.Ok(new { mensaje, data = notas });

            return Results.BadRequest(new { mensaje });
        }
            catch (Exception ex)
            {
                return Results.NotFound(new { mensaje = ex.Message });
            }
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
