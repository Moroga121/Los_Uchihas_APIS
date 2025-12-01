using MAT02_Matricula.Entities;
using MAT02_Matricula.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace MAT02_Matricula
{
    public static class MatriculaEndPoint
    {
        public static void MapMatriculaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/matricula").WithTags(nameof(MatriculaCompleta));

            group.MapGet("/", async ([FromServices] Services.IMatriculaService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    var result = await service.Obtener_Todas_Matriculas();
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener todas las Matriculas",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("GetAllMatriculas")
            .WithOpenApi();


            group.MapGet("/curso-grupo", async ([FromServices] IMatriculaService service, [FromQuery] string curso, [FromQuery] string grupo, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    // 1 Validar token

                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        return Results.Unauthorized();

                    // 2 Validar parámetros

                    if (string.IsNullOrWhiteSpace(curso) || string.IsNullOrWhiteSpace(grupo))
                    {
                        return Results.BadRequest(new { mensaje = "El curso y el grupo son obligatorios" });
                    }

                    // 3 Llamar al service CORRECTO

                    var result = await service.Obtener_Matriculados_Por_Curso_Grupo(curso, grupo);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Obtener matriculados por curso y grupo",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("GetMatriculadosPorCursoGrupo")
            .WithOpenApi();


            #region "CRUD MATRICULA ACTUALIZADO"

            // Crear matricula

            group.MapPost("/", async ([FromServices] Services.IMatriculaService service, [FromBody] Entities.Matricula matricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    // 1️ Validar token
                    var authRequest = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    authRequest.Headers.Add("access_token", accessToken);
                    var authResponse = await httpClient.SendAsync(authRequest);

                    if (!authResponse.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }

                    var prematriculaRequest = new HttpRequestMessage(HttpMethod.Get, "https://tiusr21pl.cuc-carrera-ti.ac.cr/MAT01Prematricula/Prematricula");
                    prematriculaRequest.Headers.Add("access_token", accessToken);
                    var prematriculaResponse = await httpClient.SendAsync(prematriculaRequest);

                    if (!prematriculaResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error obteniendo prematriculas" });
                    }

                    var prematriculaJson = await prematriculaResponse.Content.ReadAsStringAsync();
                    var todasPrematriculas = JsonSerializer.Deserialize<List<Prematricula>>(prematriculaJson);

                    // 3️ Validar que el estudiante tenga prematricula para este curso
                    var prematriculaEstudiante = todasPrematriculas!.FirstOrDefault(p => p.numero_identificacion == matricula.numero_identificacion && p.curso == matricula.curso);

                    if (prematriculaEstudiante == null)
                    {
                        return Results.BadRequest(new { mensaje = "El estudiante no tiene prematricula para este curso" });
                    }

                    // 4️ Validar curso usando API de cursos

                    var cursoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD3CursosAvance3/api/curso/validar?nombre={Uri.EscapeDataString(matricula.curso)}");
                    cursoRequest.Headers.Add("access_token", accessToken);
                    var cursoResponse = await httpClient.SendAsync(cursoRequest);

                    if (cursoResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "El curso no existe" });

                    }


                    if (!cursoResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando el curso" });

                    }

                    var cursoJson = await cursoResponse.Content.ReadAsStringAsync();
                    var curso = JsonSerializer.Deserialize<Curso>(cursoJson);

                    // 5️ Validar periodo futuro usando API de periodos
                    var periodoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD5PeriodosAvance3/api/periodo/validar?id={Uri.EscapeDataString(matricula.Id_periodo)}");
                    periodoRequest.Headers.Add("access_token", accessToken);
                    var periodoResponse = await httpClient.SendAsync(periodoRequest);

                    if (periodoResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "El periodo no existe" });

                    }


                    if (!periodoResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando el periodo" });

                    }

                    var periodoJson = await periodoResponse.Content.ReadAsStringAsync();
                    var periodo = JsonSerializer.Deserialize<Periodo>(periodoJson);

                    if (periodo == null)
                    {

                        return Results.BadRequest(new { mensaje = "Periodo inválido" });

                    }

                    if (periodo.Fecha_Inicio >= DateTime.Today)
                    {

                        return Results.BadRequest(new { mensaje = "Solo se pueden matricular periodos activos" });

                    }


                    var grupoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD4GruposAvance3/api/grupo/{matricula.grupo}");
                    grupoRequest.Headers.Add("access_token", accessToken);

                    var grupoResponse = await httpClient.SendAsync(grupoRequest);

                    if (grupoResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El grupo no existe" });
                    }

                    if (!grupoResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error consultando el grupo" });
                    }

                    var grupoJson = await grupoResponse.Content.ReadAsStringAsync();
                    var grupoInfo = JsonSerializer.Deserialize<Grupo>(grupoJson);

                    if (grupoInfo == null)
                    {
                        return Results.BadRequest(new { mensaje = "Error interpretando el grupo recibido" });
                    }

                    // 2. Validar que el grupo pertenece al curso

                    if (grupoInfo.ID_Curso != curso.ID_Curso && grupoInfo.ID_Curso != matricula.curso)
                    {
                        return Results.BadRequest(new { mensaje = "El grupo no pertenece al curso indicado" });
                    }

                    // 3. Validar que el periodo del grupo coincide con el de la matrícula

                    if (grupoInfo.ID_Periodo != matricula.Id_periodo)
                    {
                        return Results.BadRequest(new { mensaje = "El grupo pertenece a un periodo diferente" });
                    }


                    // 6️ Crear matrícula
                    matricula.Accion = "Crear";
                    var result = await service.CRUDMatricula(matricula);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Crear matricula",
                       descripcion: matricula,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            })
            .WithName("RealizarMatricula")
            .WithOpenApi();


            // Actualizar matricula

            group.MapPut("/", async ([FromServices] Services.IMatriculaService service, [FromBody] Entities.Matricula matricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    // 1. Validar token
                    var authRequest = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    authRequest.Headers.Add("access_token", accessToken);

                    var authResponse = await httpClient.SendAsync(authRequest);

                    if (!authResponse.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }

                    // 2. Obtener prematriculas
                    var premRequest = new HttpRequestMessage(HttpMethod.Get, "https://tiusr21pl.cuc-carrera-ti.ac.cr/MAT01Prematricula/Prematricula");
                    premRequest.Headers.Add("access_token", accessToken);

                    var premResponse = await httpClient.SendAsync(premRequest);

                    if (!premResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error obteniendo prematriculas" });

                    }

                    var prematriculas = await premResponse.Content.ReadFromJsonAsync<List<Prematricula>>();

                    // 3. Validar existencia de prematricula del estudiante en el periodo
                    var prem = prematriculas?.FirstOrDefault(p => p.numero_identificacion == matricula.numero_identificacion);

                    if (prem == null)
                    {

                        return Results.BadRequest(new { mensaje = "El estudiante no tiene prematrícula en este periodo" });

                    }


                    // 4. Validar PERIODO usando API externa (igual al POST)

                    var periodoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD5PeriodosAvance3/api/periodo/validar?id={Uri.EscapeDataString(matricula.Id_periodo)}");
                    periodoRequest.Headers.Add("access_token", accessToken);

                    var periodoResponse = await httpClient.SendAsync(periodoRequest);

                    if (periodoResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return Results.BadRequest(new { mensaje = "El periodo no existe" });
                    }

                    if (!periodoResponse.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { mensaje = "Error validando el periodo" });
                    }

                    var periodoJson = await periodoResponse.Content.ReadAsStringAsync();
                    var periodo = JsonSerializer.Deserialize<Periodo>(periodoJson);

                    if (periodo == null)
                    {

                        return Results.BadRequest(new { mensaje = "Periodo inválido" });

                    }

                    if (periodo.Fecha_Inicio <= DateTime.Today)
                    {

                        return Results.BadRequest(new { mensaje = "Solo se pueden matricular periodos futuros" });

                    }




                    // 5. VALIDAR CURSO POR NOMBRE (igual que POST)

                    var cursoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD3CursosAvance3/api/curso/validar?nombre={Uri.EscapeDataString(matricula.curso)}");
                    cursoRequest.Headers.Add("access_token", accessToken);

                    var cursoResponse = await httpClient.SendAsync(cursoRequest);

                    if (cursoResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "El curso no existe" });

                    }

                    if (!cursoResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error validando el curso" });

                    }


                    var cursoJson = await cursoResponse.Content.ReadAsStringAsync();
                    var cursoObj = JsonSerializer.Deserialize<Curso>(cursoJson);

                    if (cursoObj == null || string.IsNullOrEmpty(cursoObj.ID_Curso))
                    {

                        return Results.BadRequest(new { mensaje = "Error interpretando datos del curso" });

                    }

                    string cursoID = cursoObj.ID_Curso;



                    // 6. Obtener grupo por ID

                    var grupoRequest = new HttpRequestMessage(HttpMethod.Get, $"https://tiusr21pl.cuc-carrera-ti.ac.cr/ACD4GruposAvance3/api/grupo/{matricula.grupo}");
                    grupoRequest.Headers.Add("access_token", accessToken);

                    var grupoResponse = await httpClient.SendAsync(grupoRequest);

                    if (grupoResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        return Results.BadRequest(new { mensaje = "El grupo no existe" });

                    }

                    if (!grupoResponse.IsSuccessStatusCode)
                    {

                        return Results.BadRequest(new { mensaje = "Error obteniendo datos del grupo" });

                    }


                    var grupoInfo = await grupoResponse.Content.ReadFromJsonAsync<Grupo>();

                    if (grupoInfo == null)
                    {

                        return Results.BadRequest(new { mensaje = "Error interpretando datos del grupo" });

                    }


                    // 7. Validar que el grupo pertenece al curso (se compara con ID real)
                    if (!string.Equals(grupoInfo.ID_Curso, cursoID, StringComparison.OrdinalIgnoreCase))
                    {

                        return Results.BadRequest(new { mensaje = "El grupo no pertenece al curso indicado" });

                    }


                    // 8. Validar que el grupo pertenece al periodo
                    if (grupoInfo.ID_Periodo != matricula.Id_periodo)
                    {

                        return Results.BadRequest(new { mensaje = "El grupo pertenece a un periodo diferente" });

                    }


                    // 9. Actualizar matrícula
                    matricula.Accion = "Actualizar";

                    var result = await service.CRUDMatricula(matricula);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Actualizar matricula",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            });


            // Eliminar matricula

            group.MapDelete("/", async ([FromServices] Services.IMatriculaService service, [FromBody] Entities.Matricula matricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://tiusr21pl.cuc-carrera-ti.ac.cr/USR5Login/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    matricula.Accion = "Eliminar";
                    var result = await service.CRUDMatricula(matricula);
                    // Registrar intento exitoso en la bitacora del login
                    await service.RegistrarBitacoraAsync(
                       accion: "Eliminar matricula",
                       descripcion: result,
                       accessToken: accessToken
                   );
                    return result;
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { mensaje = ex.Message });
                }
            });



            #endregion


        }
    }
}
