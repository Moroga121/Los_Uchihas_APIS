using MAT02_Matricula.Entities;
using MAT02_Matricula.Repository;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MAT02_Matricula.Services
{
    public class MatriculaService : IMatriculaService
    {

        private readonly MatriculaRepository _matriculaRepository;
        private readonly HttpClient _httpClient;

        public MatriculaService(MatriculaRepository matriculaRepository, HttpClient httpClient)
        {
            _matriculaRepository = matriculaRepository;
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:9000/");
        }
        public async Task<IResult> CRUDMatricula(Matricula matricula)
        {

            // validaciones de datos
            if (matricula.Accion != "Eliminar")
            {
                var validacion = ValidarDatos(matricula);
                if (validacion != null)
                {
                    return validacion;
                }
            }
            var (creada, mensajeSP) = await _matriculaRepository.CRUDMatricula(matricula);

            await RegistrarBitacoraAsync(
                usuario: "usuario_actual",
                accion: matricula.Accion,
                descripcion: JsonSerializer.Serialize(matricula)
            );
            var data = creada ?? matricula;

            if (mensajeSP.Contains("Matrícula creada exitosamente"))
            {

                return Results.Created($"/Matricula/{data.Id_matricula}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }

            if (mensajeSP.Contains("Matrícula actualizada exitosamente"))
            {
                return Results.Created($"/Matricula/{data.Id_matricula}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }
            if (mensajeSP.Contains("Matrícula eliminada exitosamente"))
            {
                return Results.Ok(new { mensaje = mensajeSP });
            }

            return Results.BadRequest(new { mensaje = mensajeSP });

        }
        public async Task<IEnumerable<MatriculaCompleta>> Obtener_Todas_Matriculas()
        {
            return await _matriculaRepository.Obtener_Todas_Matriculas();
        }
      
        public IResult? ValidarDatos(Matricula matricula)
        {
            if (string.IsNullOrEmpty(matricula.numero_identificacion))
                return Results.BadRequest("El número de identificación es obligatorio.");
            if (string.IsNullOrEmpty(matricula.grupo))
                return Results.BadRequest("El grupo es obligatorio.");
            if (string.IsNullOrEmpty(matricula.curso))
                return Results.BadRequest("El curso es obligatorio.");
            if (!string.IsNullOrEmpty(matricula.numero_identificacion) && matricula.numero_identificacion.Length > 22)
                return Results.BadRequest("El número de identificación no puede exceder 22 caracteres.");
            if (!string.IsNullOrEmpty(matricula.curso) && matricula.curso.Length > 50)
                return Results.BadRequest("El nombre del curso no puede exceder 50 caracteres.");
            if (!string.IsNullOrEmpty(matricula.grupo) && matricula.grupo.Length > 20)
                return Results.BadRequest("El nombre del grupo no puede exceder 20 caracteres.");

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
