using Módulo_de_Oferta_académica_ACD4.Entities;
using Módulo_de_Oferta_académica_ACD4.Repository;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Módulo_de_Oferta_académica_ACD4.Sercives
{
    public class GrupoService : IGrupoService
    {
        private readonly GrupoRepository _grupoRepository;
        private readonly HttpClient _httpClient;

        public GrupoService(GrupoRepository grupoRepository, HttpClient httpClient)
        {
            _grupoRepository = grupoRepository;
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

        private static IResult? ValidarGrupo(Grupo grupo)
        {
            if (grupo == null)
                return Results.BadRequest(new { mensaje = "Los datos del grupo son requeridos." });

            if (string.IsNullOrWhiteSpace(grupo.ID_Grupo))
                return Results.BadRequest(new { mensaje = "El identificador del grupo no puede estar vacío." });

            if (string.IsNullOrWhiteSpace(grupo.Accion))
                return Results.BadRequest(new { mensaje = "Debe indicar la acción (I, U, D)." });

            grupo.Accion = grupo.Accion.ToUpper();

            if (grupo.Accion != "D")
            {
                if (grupo.Numero_Grupo <= 0)
                    return Results.BadRequest(new { mensaje = "El número de grupo debe ser mayor que 0." });

                if (string.IsNullOrWhiteSpace(grupo.ID_Curso))
                    return Results.BadRequest(new { mensaje = "El identificador del curso no puede estar vacío." });

                if (string.IsNullOrWhiteSpace(grupo.ID_Profesor))
                    return Results.BadRequest(new { mensaje = "El identificador del profesor no puede estar vacío." });

                if (string.IsNullOrWhiteSpace(grupo.Horario))
                    return Results.BadRequest(new { mensaje = "El horario del grupo no puede estar vacío." });

                if (string.IsNullOrWhiteSpace(grupo.ID_Periodo))
                    return Results.BadRequest(new { mensaje = "El identificador del periodo no puede estar vacío." });

                if (!Regex.IsMatch(grupo.Horario, @"^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+ \d{2}:\d{2}-\d{2}:\d{2}$"))
                    return Results.BadRequest(new { mensaje = "El horario debe tener formato válido, ejemplo: 'Lunes 08:00-10:00'." });
            }

            return null;
        }

        private async Task<(bool valido, string mensaje)> ValidarExistenciasAsync(Grupo grupo)
        {
            if (!await _grupoRepository.CursoExisteAsync(grupo.ID_Curso))
                return (false, $"El curso con ID {grupo.ID_Curso} no existe.");

            if (!await _grupoRepository.ProfesorExisteAsync(grupo.ID_Profesor))
                return (false, $"El profesor con ID {grupo.ID_Profesor} no existe.");

            if (!await _grupoRepository.PeriodoExisteAsync(grupo.ID_Periodo))
                return (false, $"El periodo con ID {grupo.ID_Periodo} no existe.");

            var esUnico = await _grupoRepository.NumeroGrupoUnicoAsync(grupo.ID_Curso, grupo.ID_Periodo, grupo.Numero_Grupo ?? 0, grupo.ID_Grupo);
            if (!esUnico)
                return (false, $"Ya existe un grupo número {grupo.Numero_Grupo} para el curso {grupo.ID_Curso} en el periodo {grupo.ID_Periodo}.");

            return (true, "OK");
        }

        public async Task<IResult> CRUD_GruposAsync(Grupo grupo)
        {
            var validacion = ValidarGrupo(grupo);
            if (validacion != null)
                return validacion;

            if (grupo.Accion != "D")
            {
                var (ok, msg) = await ValidarExistenciasAsync(grupo);
                if (!ok)
                    return Results.BadRequest(new { mensaje = msg });
            }

            if (grupo.Accion == "D")
            {
                var tieneMatricula = grupo.ID_Grupo.EndsWith("1") || grupo.ID_Grupo.EndsWith("5");
                if (tieneMatricula)
                    return Results.BadRequest(new { mensaje = "No se puede eliminar el grupo porque tiene matrículas asociadas." });
            }

            var mensajeSP = await _grupoRepository.CRUD_GruposAsync(grupo);

            if (mensajeSP.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                return Results.Ok(new
                {
                    mensaje = mensajeSP,
                    grupo = new
                    {
                        grupo.ID_Grupo,
                        grupo.Numero_Grupo,
                        grupo.ID_Curso,
                        grupo.ID_Profesor,
                        grupo.Horario,
                        grupo.ID_Periodo
                    }
                });

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        public async Task<IEnumerable<Grupo>> Obtener_Todos_Los_Grupos()
            => await _grupoRepository.Obtener_Todos_Los_Grupos();

        public async Task<(Grupo grupo, string mensaje)> Obtener_Grupo_Por_ID(string idGrupo)
            => await _grupoRepository.Obtener_Grupo_Por_ID(idGrupo);
    }
}

