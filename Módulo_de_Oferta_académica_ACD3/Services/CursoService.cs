using Módulo_de_Oferta_académica_ACD3.Entities;
using Módulo_de_Oferta_académica_ACD3.Repository;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Módulo_de_Oferta_académica_ACD3.Services
{
    public class CursoService : ICursoService
    {
        private readonly CursoRepository _cursoRepository;
        private readonly HttpClient _httpClient;

        public CursoService(CursoRepository cursoRepository, HttpClient httpClient)
        {
            _cursoRepository = cursoRepository;
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

        public async Task<IEnumerable<Curso>> Obtener_Todos_Los_Cursos()
            => await _cursoRepository.Obtener_Todos_Los_Cursos();

        public async Task<(Curso curso, string mensaje)> Obtener_Curso_Por_ID(string idCurso)
            => await _cursoRepository.Obtener_Curso_Por_ID(idCurso);

        public async Task<IEnumerable<Curso>> Obtener_Cursos_Por_Carrera(string idCarrera)
            => await _cursoRepository.Obtener_Cursos_Por_Carrera(idCarrera);

        private IResult? ValidarCurso(Curso curso)
        {
            if (curso == null)
                return Results.BadRequest(new { mensaje = "Los datos del curso son requeridos." });

            if (string.IsNullOrWhiteSpace(curso.ID_Curso))
                return Results.BadRequest(new { mensaje = "El identificador del curso no puede estar vacío." });

            if (string.IsNullOrWhiteSpace(curso.Accion))
                return Results.BadRequest(new { mensaje = "Debe indicar la acción a realizar (I, U o D)." });

            curso.Accion = curso.Accion.Trim().ToUpper();

            if (curso.Accion != "I" && curso.Accion != "U" && curso.Accion != "D")
                return Results.BadRequest(new { mensaje = "La acción debe ser 'I', 'U' o 'D'." });

            if (curso.Accion != "D")
            {
                if (string.IsNullOrWhiteSpace(curso.ID_Carrera))
                    return Results.BadRequest(new { mensaje = "El identificador de la carrera no puede estar vacío." });

                if (curso.Nivel <= 0 || curso.Nivel > 12)
                    return Results.BadRequest(new { mensaje = "El nivel debe estar entre 1 y 12." });

                if (string.IsNullOrWhiteSpace(curso.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre del curso no puede estar vacío." });

                if (!Regex.IsMatch(curso.Nombre, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$"))
                    return Results.BadRequest(new { mensaje = "El nombre del curso solo puede contener letras y espacios." });
            }

            return null;
        }

        public async Task<IResult> CRUD_CursosAsync(Curso curso)
        {
            var validacion = ValidarCurso(curso);
            if (validacion != null)
                return validacion;

            try
            {
                var mensajeSP = await _cursoRepository.CRUD_CursosAsync(curso);

                if (mensajeSP.Contains("registrado correctamente"))
                    return Results.Created($"/api/curso/{curso.ID_Curso}", new 
                    { 
                        mensaje = mensajeSP,
                        curso = new {

                            curso.ID_Curso,
                            curso.ID_Carrera,
                            curso.Nivel,
                            curso.Nombre

                        }
                    });

                if (mensajeSP.Contains("actualizado correctamente") || mensajeSP.Contains("eliminado correctamente"))
                    return Results.Ok(new 
                    { 
                        mensaje = mensajeSP,
                        curso = new {

                            curso.ID_Curso,
                            curso.ID_Carrera,
                            curso.Nivel,
                            curso.Nombre
                            
                        }
                    });

                if (mensajeSP.Contains("No se encontró"))
                    return Results.NotFound(new { mensaje = mensajeSP });

                if (mensajeSP.Contains("Ya existe"))
                    return Results.Conflict(new { mensaje = mensajeSP });

                return Results.BadRequest(new { mensaje = mensajeSP });
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Message.Contains("a foreign key constraint fails"))
                {
                    return Results.BadRequest(new
                    {
                        mensaje = "No se puede registrar o actualizar el curso porque la carrera asociada no existe."
                    });
                }

                if (ex.Message.Contains("Duplicate entry"))
                {
                    return Results.Conflict(new
                    {
                        mensaje = "Ya existe un curso con ese identificador."
                    });
                }

                return Results.Problem($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Ocurrió un error al procesar la solicitud: {ex.Message}");
            }
        }
    }
}


