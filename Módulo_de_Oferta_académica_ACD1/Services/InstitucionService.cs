using Microsoft.AspNetCore.Http;
using Módulo_de_Oferta_académica_ACD1.Entities;
using Módulo_de_Oferta_académica_ACD1.Repository;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Módulo_de_Oferta_académica_ACD1.Services
{
    public class InstitucionService : IInstitucionService
    {
        private readonly InstitucionRepository _institucionRepository;
        private readonly HttpClient _httpClient;

        public InstitucionService(InstitucionRepository institucionRepository, HttpClient httpClient)
        {
            _institucionRepository = institucionRepository;
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

        public async Task<IEnumerable<Institucion>> Obtener_Todas_Las_Instituciones()
        {
            return await _institucionRepository.Obtener_Todas_Las_Instituciones();
        }

        public async Task<(Institucion institucion, string mensaje)> Obtener_Institucion_Por_ID(string idInstitucion)
        {
            if (string.IsNullOrWhiteSpace(idInstitucion))
                return (null, "El identificador de la institución no puede estar vacío.");

            return await _institucionRepository.Obtener_Institucion_Por_ID(idInstitucion);
        }

        public IResult? ValidarInstitucion(Institucion institucion)
        {
            if (institucion == null)
                return Results.BadRequest(new { mensaje = "Los datos de la institución son requeridos." });

            if (string.IsNullOrWhiteSpace(institucion.ID_Institucion))
                return Results.BadRequest(new { mensaje = "El identificador de la institución no puede estar vacío." });

            if (string.IsNullOrWhiteSpace(institucion.Accion))
                return Results.BadRequest(new { mensaje = "Debe indicar la acción a realizar: I (Insertar), U (Actualizar), D (Eliminar)." });

            institucion.Accion = institucion.Accion.ToUpper();

            if (institucion.Accion != "I" && institucion.Accion != "U" && institucion.Accion != "D")
                return Results.BadRequest(new { mensaje = "Acción no válida. Use I, U o D." });

            if (institucion.Accion != "D")
            {
                if (string.IsNullOrWhiteSpace(institucion.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre de la institución no puede estar vacío." });

                if (!Regex.IsMatch(institucion.Nombre, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$"))
                    return Results.BadRequest(new { mensaje = "El nombre de la institución solo puede contener letras y espacios." });
            }

            return null;
        }

        public async Task<IResult> CRUD_InstitucionesAsync(Institucion institucion)
        {
            var validacion = ValidarInstitucion(institucion);
            if (validacion != null)
                return validacion;

            var mensajeSP = await _institucionRepository.CRUD_InstitucionesAsync(institucion);

            if (mensajeSP.Contains("registrada correctamente"))
            {
                return Results.Created($"/api/institucion/{institucion.ID_Institucion}", new 
                { 

                    mensaje = mensajeSP,
                    institucion = new
                    {
                        institucion.ID_Institucion,
                        institucion.Nombre
                    }

                });
            }

            if (mensajeSP.Contains("actualizada correctamente"))
            {
                return Results.Ok(new
                {

                    mensaje = mensajeSP,
                    institucion = new
                    {
                        institucion.ID_Institucion,
                        institucion.Nombre
                    }

                });
            }

            if (mensajeSP.Contains("Institución eliminada correctamente."))
            {
                return Results.Ok(new
                {

                    mensaje = mensajeSP,
                    institucion = new
                    {
                        institucion.ID_Institucion,
                        institucion.Nombre
                    }

                });
            }


            if (mensajeSP.Contains("No se encontró"))
            {
                return Results.NotFound(new { mensaje = mensajeSP });
            }

            if (mensajeSP.Contains("Ya existe"))
            {
                return Results.Conflict(new { mensaje = mensajeSP });
            }

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        public async Task<IEnumerable<Institucion>> Buscar_Instituciones_Por_Nombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return await _institucionRepository.Obtener_Todas_Las_Instituciones();
            }

            return await _institucionRepository.Buscar_Instituciones_Por_Nombre(nombre);
        }
    }
}


