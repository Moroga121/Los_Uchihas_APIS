using MAT02_Matricula.Entities;
using Microsoft.AspNetCore.Mvc;
using MAT02_Matricula.Services;


namespace MAT02_Matricula
{
    public static class MatriculaEndPoint
    {
        public static void MapMatriculaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/matricula").WithTags(nameof(MatriculaCompleta));

            group.MapGet("/", async ([FromServices] Services.IMatriculaService service, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    var result = await service.Obtener_Todas_Matriculas();
                    return Results.Ok(result);
              
            })
            .WithName("GetAllMatriculas")
            .WithOpenApi();


            // Crear matricula
            group.MapPost("/", async (
                [FromServices] Services.IMatriculaService  service,
                [FromBody] Entities.Matricula matricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
               
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    matricula.Accion = "Crear";
                    var result = await service.CRUDMatricula(matricula);
                    return result;
               
            })
            .WithName("RealizarMatricula")
            .WithOpenApi();
            // Actualizar matricula
            group.MapPut("/", async (
             [FromServices] Services.IMatriculaService service,
             [FromBody] Entities.Matricula matricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    matricula.Accion = "Actualizar";
                    var result = await service.CRUDMatricula(matricula);
                    return result;
                
              
            })
         .WithName("UpdateMatricula")
         .WithOpenApi();

            // Eliminar matricula
            group.MapDelete("/", async (
             [FromServices] Services.IMatriculaService service,
             [FromBody] Entities.Matricula matricula, [FromHeader(Name = "access_token")] string accessToken, HttpClient httpClient) =>
            {
                 var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/login/validate");
                    request.Headers.Add("access_token", accessToken);

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        return Results.Unauthorized();

                    }
                    matricula.Accion = "Eliminar";
                    var result = await service.CRUDMatricula(matricula);
                    return Results.Ok(result);
               
            });



        }
    }
}
