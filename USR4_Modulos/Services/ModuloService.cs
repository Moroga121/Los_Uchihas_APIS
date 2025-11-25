using System.Text.Json;
using System.Text.RegularExpressions;
using USR4_Modulos.Entities;
using USR4_Modulos.Repository;
namespace USR4_Modulos.Services
{
    public class ModuloService : IModulosService
    {
        private readonly ModuloRepository _moduloRepository;
        private readonly HttpClient _httpClient = new HttpClient();

        public ModuloService(ModuloRepository moduloRepository, HttpClient httpClient)
        {
            _moduloRepository = moduloRepository;
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


        #region "Get Modulos"

        #region "Obrtener Modulo por ID"

        public async Task<(Modulos modulo, string mensaje)> Obtener_Modulo_Por_ID(string identificador_modulo)
        {
            var (parametro, mensaje) = await _moduloRepository.Obtener_Modulo_Por_ID(identificador_modulo);
            return (parametro, mensaje);
        }

        #endregion

        #region "Obtener Todos Los Modulos"

        public async Task<IEnumerable<Modulos>> Obtener_Todos_Los_Modulos()
        {


            var usuarios = await _moduloRepository.Obtener_Todos_Los_Modulos();

            return usuarios;
        }

        #endregion
        #region "Obtener Todos Los roles y modulos relacionados"

        public async Task<IEnumerable<Rol_Modulo>> Obtener_Todos_Los_Rol_Usuario()
        {


            var rol_usaurios = await _moduloRepository.Obtener_Todos_Los_Rol_Usuario();

            return rol_usaurios;
        }

        #endregion
        #region "Obrtener Modulo por rol"

        public async Task<(IEnumerable<Modulos>, string mensaje)> Obtener_Modulo_Por_Rol(string rol)
        {
            var (parametro, mensaje) = await _moduloRepository.Obtener_Modulo_Por_Rol(rol);
            return (parametro, mensaje);
        }

        #endregion

        #endregion

        #region "Método Validaciones"
        public IResult? ValidarModulos(Modulos modulo)
        {
            if (modulo.Accion == "Insert" || modulo.Accion == "Update")
            {
                if (string.IsNullOrWhiteSpace(modulo.Identificador_Modulo))
                {
                    return Results.BadRequest(new { mensaje = "El identificador del modulo no puede estar vacío ni tener espacios en blanco." });
                }

                if (string.IsNullOrWhiteSpace(modulo.Nombre_Modulo))
                {
                    return Results.BadRequest(new { mensaje = "El nombre del modulo no puede estar vacío ni tener espacios en blanco." });
                }

                if (!Regex.IsMatch(modulo.Nombre_Modulo, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$"))
                {
                    return Results.BadRequest(new { mensaje = "El nombre del modulo solo puede contener letras y espacios." });
                }
            }
            return null;
        }
        #endregion

        #region "CRUD Modulos"
        public async Task<IResult> CRUD_ModulosAsync(Modulos modulo)
        {

            if (modulo.Accion != "Delete")
            {
                var validacion = ValidarModulos(modulo);
                if (validacion != null)
                {
                    return validacion;
                }
            }

            
            if (modulo.Accion == "Delete")
            {
                
                var (moduloExistente, mensajeBusq) = await _moduloRepository.Obtener_Modulo_Por_ID(modulo.Identificador_Modulo);

                if (moduloExistente == null)
                {
                    return Results.NotFound(new { mensaje = "No se encontró el módulo a eliminar." });
                }

                
                var mensajeElim = await _moduloRepository.CRUD_ModulosAsync(modulo);

                if (mensajeElim.Contains("Modulo eliminado correctamente"))
                {
                    return Results.Ok(new
                    {
                        mensaje = mensajeElim,
                        modulo = new
                        {
                            moduloExistente.Identificador_Modulo,
                            moduloExistente.Nombre_Modulo
                        }
                    });
                }

                if (mensajeElim.Contains("no existe"))
                {
                    return Results.NotFound(new { mensaje = mensajeElim });
                }

                return Results.BadRequest(new { mensaje = mensajeElim });
            }

           
            var mensajeSP = await _moduloRepository.CRUD_ModulosAsync(modulo);

            
            if (mensajeSP.Contains("Modulo creado correctamente"))
            {
                return Results.Created($"/modulo/{modulo.Identificador_Modulo}", new
                {
                    mensaje = mensajeSP,
                    modulo = new
                    {
                        modulo.Identificador_Modulo,
                        modulo.Nombre_Modulo
                    }
                });
            }

            
            if (mensajeSP.Contains("Modulo actualizado correctamente"))
            {
                return Results.Ok(new
                {
                    mensaje = mensajeSP,
                    modulo = new
                    {
                        modulo.Identificador_Modulo,
                        modulo.Nombre_Modulo
                    }
                });
            }

            
            if (mensajeSP.Contains("no existe"))
            {
                return Results.NotFound(new { mensaje = mensajeSP });
            }

            
            if (mensajeSP.Contains("Ya existe"))
            {
                return Results.Conflict(new { mensaje = mensajeSP });
            }

            
            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        #endregion

        #region Actualizar permisos con módulos
        public async Task<IResult> ActualizarPermisosAsync(string moduloId, List<string> rolesAsignados)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(moduloId))
            {
                return Results.BadRequest(new { mensaje = "El identificador del modulo es obligatorio." });
            }

            if (rolesAsignados == null || rolesAsignados.Count == 0)
            {
                return Results.BadRequest(new { mensaje = "Debe seleccionar al menos un rol para asignar al modulo." });
            }

            // Verificar si el módulo existe
            var (moduloExistente, mensajeBusq) = await _moduloRepository.Obtener_Modulo_Por_ID(moduloId);
            if (moduloExistente == null)
            {
                return Results.NotFound(new { mensaje = "No se encontró el modulo especificado." });
            }

            // Llamar al procedimiento almacenado para actualizar permisos
            var mensajeSP = await _moduloRepository.ActualizarPermisosModuloAsync(moduloId, rolesAsignados);

            if (mensajeSP.Contains("Permisos actualizados correctamente."))
            {
                return Results.Created($"/modulo/{moduloExistente.Identificador_Modulo}", new
                {
                    mensaje = mensajeSP,
                    modulo = new
                    {
                        moduloExistente.Identificador_Modulo,
                        moduloExistente.Nombre_Modulo,
                        RolesAsignados = rolesAsignados
                    }
                });
            }
            else
            {
                return Results.BadRequest(new { mensaje = mensajeSP });
            }
        }
        #endregion
    }
}
