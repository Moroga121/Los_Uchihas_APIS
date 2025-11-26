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
                _httpClient.BaseAddress = new Uri("http://localhost:9000/");
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

            await RegistrarBitacoraAsync(
             usuario: "usuario_actual",
             accion: notas.Accion,
             descripcion: JsonSerializer.Serialize(notas)
            );

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
            if (string.IsNullOrWhiteSpace(grupo))
                return Results.BadRequest(new { mensaje = "El grupo es obligatorio." });
            if (grupo.Length > 20)
                return Results.BadRequest(new { mensaje = "El grupo no puede superar los 20 caracteres." });

            if (string.IsNullOrWhiteSpace(curso))
                return Results.BadRequest(new { mensaje = "El curso es obligatorio." });
            if (curso.Length > 50)
                return Results.BadRequest(new { mensaje = "El curso no puede superar los 50 caracteres." });

            var (desgloserubro, mensaje) = await _notasRepository.Obtener_Desglose_Por_ID(grupo, curso);

            if (desgloserubro != null)
                return Results.Ok(new { mensaje, data = desgloserubro });

            return Results.NotFound(new { mensaje });
        }

        public async Task<IResult> Obtener_Notas_By_Id(string numero_identificacion, string curso)
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

            if (notas != null && notas.Any())
                return Results.Ok(new { mensaje, data = notas });

            return Results.NotFound(new { mensaje });
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
