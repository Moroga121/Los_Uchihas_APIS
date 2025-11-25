using Módulo_de_Oferta_académica_ACD6.Entities;
using Módulo_de_Oferta_académica_ACD6.Repository;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Módulo_de_Oferta_académica_ACD6.Services
{
    public class ProfesorService : IProfesorService
    {
        private readonly ProfesorRepository _profesorRepository;
        private readonly string _dominioPermitido;
        private readonly HttpClient _httpClient;

        public ProfesorService(ProfesorRepository profesorRepository, HttpClient httpClient, IConfiguration config)
        {
            _profesorRepository = profesorRepository;
            _httpClient = httpClient;
            _profesorRepository = profesorRepository;
            _dominioPermitido = config["ConfiguracionGeneral:DominioEmailPermitido"] ?? "cuc.ac.cr";
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

        private IResult? ValidarProfesor(Profesor profesor)
        {
            if (string.IsNullOrWhiteSpace(profesor.ID_Profesor))
                return Results.BadRequest(new { exito = false, mensaje = "El identificador del profesor no puede estar vacío." });

            if (profesor.Accion == "D")
                return null;

            if (string.IsNullOrWhiteSpace(profesor.TipoIdentificacion) ||
                string.IsNullOrWhiteSpace(profesor.NumeroIdentificacion) ||
                string.IsNullOrWhiteSpace(profesor.Nombre) ||
                string.IsNullOrWhiteSpace(profesor.Email) ||
                string.IsNullOrWhiteSpace(profesor.Telefono))
                return Results.BadRequest(new { exito = false, mensaje = "Todos los campos son obligatorios." });

            // Permite tildes y letras de cualquier idioma
            if (!Regex.IsMatch(profesor.Nombre, @"^[\p{L} ]+$"))
                return Results.BadRequest(new { exito = false, mensaje = "El nombre solo puede contener letras y espacios." });

            if (!Regex.IsMatch(profesor.Telefono, @"^[2-9][0-9]{3}-?[0-9]{4}$"))
                return Results.BadRequest(new { exito = false, mensaje = "El teléfono debe tener formato local (####-#### o ########)." });

            var edad = DateTime.Today.Year - profesor.FechaNacimiento.Year;
            if (profesor.FechaNacimiento > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 18)
                return Results.BadRequest(new { exito = false, mensaje = "El profesor debe ser mayor de edad." });

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(profesor.Email))
                return Results.BadRequest(new { exito = false, mensaje = "Formato de correo inválido." });

            if (!profesor.Email.EndsWith(_dominioPermitido, StringComparison.OrdinalIgnoreCase))
                return Results.BadRequest(new { exito = false, mensaje = $"El correo debe pertenecer al dominio {_dominioPermitido}." });

            return null;
        }

        public async Task<IResult> CRUD_ProfesoresAsync(Profesor profesor)
        {
            if (profesor.Accion == "D")
            {
                if (string.IsNullOrWhiteSpace(profesor.ID_Profesor))
                    return Results.BadRequest(new { exito = false, mensaje = "El ID del profesor es obligatorio para eliminar." });

                var (profExistente, _) = await _profesorRepository.Obtener_Por_ID(profesor.ID_Profesor);
                if (profExistente == null)
                    return Results.NotFound(new { exito = false, mensaje = "No se encontró un profesor con ese ID." });

                var mensajeDelete = await _profesorRepository.CRUD_ProfesoresAsync(profesor);

                if (mensajeDelete.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                    return Results.Ok(new
                    {
                        exito = true,
                        mensaje = mensajeDelete
                    });

                return Results.BadRequest(new { exito = false, mensaje = mensajeDelete });
            }

            var validacion = ValidarProfesor(profesor);
            if (validacion != null) return validacion;

            if (profesor.Accion == "I")
            {
                var (profExistente, _) = await _profesorRepository.Obtener_Por_ID(profesor.ID_Profesor);
                if (profExistente != null)
                    return Results.BadRequest(new { exito = false, mensaje = $"El profesor con ID {profesor.ID_Profesor} ya existe." });
            }

            if (profesor.FechaNacimiento == default)
                return Results.BadRequest(new { exito = false, mensaje = "La fecha de nacimiento no tiene un formato válido." });

            var mensaje = await _profesorRepository.CRUD_ProfesoresAsync(profesor);

            if (mensaje.Contains("correctamente", StringComparison.OrdinalIgnoreCase))
                return Results.Ok(new
                {
                    exito = true,
                    mensaje,
                    profesor = new
                    {
                        profesor.ID_Profesor,
                        profesor.TipoIdentificacion,
                        profesor.NumeroIdentificacion,
                        profesor.Nombre,
                        profesor.Email,
                        profesor.FechaNacimiento,
                        profesor.Telefono
                    }
                });

            return Results.BadRequest(new { exito = false, mensaje });
        }

        public async Task<IEnumerable<Profesor>> Obtener_Todos()
            => await _profesorRepository.Obtener_Todos();

        public async Task<(Profesor? profesor, string mensaje)> Obtener_Por_ID(string id)
            => await _profesorRepository.Obtener_Por_ID(id);

        public async Task<IEnumerable<Profesor>> BuscarProfesoresAsync(string busqueda, string ordenCampo, string ordenDireccion, int pagina, int tamanoPagina)
            => await _profesorRepository.BuscarProfesoresAsync(busqueda, ordenCampo, ordenDireccion, pagina, tamanoPagina);

    }
}

