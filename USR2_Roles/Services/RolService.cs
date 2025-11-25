using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using USR2_Roles.Entities;
using USR2_Roles.Repository;

namespace USR2_Roles.Services
{
    public class RolService : IRolService

    {
        private readonly RolesRepository _rolRepository;
        private readonly HttpClient _httpClient;

        public RolService(RolesRepository rolRepository, HttpClient httpClient)
        {
            _rolRepository = rolRepository;
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

        #region "Obtener Todos Los Roles"

        public async Task<IEnumerable<Rol>> Obtener_Todos_Los_Roles()
        {

            var usuarios = await _rolRepository.Obtener_Todos_Los_Usuarios();

            return usuarios;
        }

        #endregion

        #region "Obrtener Rol por ID"

        public async Task<(Rol rol, string mensaje)> Obtener_Rol_Por_ID(string identificador_rol)
        {
            var (rol, mensaje) = await _rolRepository.Obtener_Rol_Por_ID(identificador_rol);
            return (rol, mensaje);
        }

        #endregion

        #region "CRUD Usuarios"

        #region "Método Validaciones"

        public IResult? ValidarRol(Rol rol)
        {

            if (rol.Accion == "Insert" || rol.Accion == "Update")
            {
                rol.Identificador_Rol = rol.Identificador_Rol.Trim();
                rol.Nombre_Rol = rol.Nombre_Rol.Trim();

                if (rol.Identificador_Rol.Length > 3)
                {

                    return Results.BadRequest(new { mensaje = "El identificador del rol no puede tener más de 3 caracteres." });

                }

                if (string.IsNullOrWhiteSpace(rol.Identificador_Rol))
                {
                    return Results.BadRequest(new { mensaje = "El identificador del rol no puede estar vacío." });
                }

                if (string.IsNullOrWhiteSpace(rol.Nombre_Rol))
                {
                    return Results.BadRequest(new { mensaje = "El nombre del rol no puede estar vacío." });
                }

                if (!Regex.IsMatch(rol.Nombre_Rol, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    return Results.BadRequest(new { mensaje = "El nombre del rol solo puede contener letras y espacios." });
                }

                if (rol.Identificador_Rol.Length > 3)
                {
                    return Results.BadRequest(new { mensaje = "El identificador del rol no puede tener más de 3 caracteres." });
                }

            }
            return null;

        }

        #endregion



        public async Task<IResult> CRUD_RolesAsync(Rol rol)
        {

            // LLamamos a la Validación

            if (rol.Accion != "Delete")
            {
                var validacion = ValidarRol(rol);
                if (validacion != null)
                {
                    return validacion;
                }
            }

            if (rol.Accion == "Delete")
            {

                var (rolExistente, mensajeBusq) = await _rolRepository.Obtener_Rol_Por_ID(rol.Identificador_Rol);


                if (rolExistente == null)
                {
                    return Results.NotFound(new { mensaje = "No se encontró el rol a eliminar." });
                }


                var mensajeElim = await _rolRepository.CRUD_RolesAsync(rol);


                if (mensajeElim.Contains("Rol eliminado correctamente"))
                {
                    return Results.Ok(new
                    {
                        mensaje = mensajeElim,
                        rol = new
                        {
                            rolExistente.Identificador_Rol,
                            rolExistente.Nombre_Rol
                        }
                    });
                }


                if (mensajeElim.Contains("no existe"))
                {
                    return Results.NotFound(new { mensaje = mensajeElim });
                }


                return Results.BadRequest(new { mensaje = mensajeElim });
            }


            var mensajeSP = await _rolRepository.CRUD_RolesAsync(rol);

            if (mensajeSP.Contains("Rol creado correctamente"))
            {
                return Results.Created($"/rol/{rol.Identificador_Rol}", new
                {
                    mensaje = mensajeSP,
                    rol = new
                    {
                        rol.Identificador_Rol,
                        rol.Nombre_Rol
                    }
                });
            }

            if (mensajeSP.Contains("Rol actualizado correctamente"))
            {
                return Results.Ok(new
                {
                    mensaje = mensajeSP,
                    rol = new
                    {
                        rol.Identificador_Rol,
                        rol.Nombre_Rol
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

        #region Acualizar permisos con modulos
        public async Task<IResult> ActualizarPermisosAsync(string rolId, List<string> modulos)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(rolId))
            {
                return Results.BadRequest(new { mensaje = "El identificador del rol es obligatorio." });
            }

            if (modulos == null || modulos.Count == 0)
            {
                return Results.BadRequest(new { mensaje = "Debe seleccionar al menos un modulo para asignar." });
            }

            // Verificar si el rol existe
            var (rolExistente, mensajeBusq) = await _rolRepository.Obtener_Rol_Por_ID(rolId);

            if (rolExistente == null)
            {
                return Results.NotFound(new { mensaje = "No se encontro el rol especificado." });
            }

            // Llamar al procedimiento almacenado
            var mensajeSP = await _rolRepository.ActualizarPermisosRolAsync(rolId, modulos);

            if (mensajeSP.Contains("Permisos actualizados correctamente."))
            {
                return Results.Created($"/rol/{rolExistente.Identificador_Rol}", new
                {
                    mensaje = mensajeSP,
                    rol = new
                    {
                        rolExistente.Identificador_Rol,
                        rolExistente.Nombre_Rol,
                        ModulosAsignados = modulos
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
