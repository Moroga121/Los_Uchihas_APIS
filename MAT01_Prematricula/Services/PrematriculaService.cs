using MAT01_Prematricula.Entities;
using MAT01_Prematricula.Repository;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MAT01_Prematricula.Services
{
    public class PrematriculaService : IPrematriculaService
    {

        private readonly PrematriculaRepository _prematriculaRepository;
        private readonly HttpClient _httpClient;

        public PrematriculaService(PrematriculaRepository prematriculaRepository, HttpClient httpClient)
        {
            _prematriculaRepository = prematriculaRepository;
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:9000/");
        }
        public async Task<IResult> CRUDPrematricula(Prematricula prematricula)
        {
            // validaciones
            if (prematricula.Accion != "Eliminar")
            {
                var validacion = ValidarDatos(prematricula);
                if (validacion != null)
                    return validacion;
            }

            var (creada, mensajeSP) = await _prematriculaRepository.CRUDPrematricula(prematricula);


            var data = creada ?? prematricula;

            if (mensajeSP.Contains("Prematrícula creada exitosamente"))
            {

                return Results.Created($"/Prematricula/{data.id_prematricula}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }

            if (mensajeSP.Contains("Prematrícula actualizada exitosamente"))
            {
                return Results.Created($"/Prematricula/{data.id_prematricula}", new
                {
                    mensaje = mensajeSP,
                    data
                });
            }
            if (mensajeSP.Contains("Prematrícula eliminada exitosamente"))
            {
                return Results.Ok(new { mensaje = mensajeSP });
            }

            return Results.BadRequest(new { mensaje = mensajeSP });
        }

        public async Task<IEnumerable<Prematricula>> Obtener_Todas_Prematriculas()
        {
            var prematriculas = await _prematriculaRepository.Obtener_Todas_Prematriculas();

            return prematriculas;
        }
        public async Task<(Prematricula prematricula, string mensaje)> Obtener_Prematricula_Por_ID(string Id_Prematricula)
        {
            var prematriculas = await _prematriculaRepository.Obtener_Prematricula_Por_ID(Id_Prematricula);

            return prematriculas;
        }
        public IResult? ValidarDatos(Prematricula prematricula)
        {
            if (string.IsNullOrEmpty(prematricula.numero_identificacion))
                return Results.BadRequest("El número de identificación es obligatorio.");
            if (string.IsNullOrEmpty(prematricula.carrera))
                return Results.BadRequest("La carrera es obligatoria.");
            if (string.IsNullOrEmpty(prematricula.curso))
                return Results.BadRequest("El curso es obligatorio.");
            if (string.IsNullOrEmpty(prematricula.Id_Periodo))
                return Results.BadRequest("El ID del período es obligatorio.");
            if (!string.IsNullOrEmpty(prematricula.numero_identificacion) && prematricula.numero_identificacion.Length > 22)
                return Results.BadRequest("El número de identificación no puede exceder 22 caracteres.");
            if (!string.IsNullOrEmpty(prematricula.carrera) && prematricula.carrera.Length > 100)
                return Results.BadRequest("El nombre de la carrera no puede exceder 100 caracteres.");
            if (!string.IsNullOrEmpty(prematricula.curso) && prematricula.curso.Length > 50)
                return Results.BadRequest("El nombre del curso no puede exceder 50 caracteres.");
            if (!string.IsNullOrEmpty(prematricula.observaciones) && prematricula.observaciones.Length > 255)
                return Results.BadRequest("Las observaciones no pueden exceder 255 caracteres.");

            return null;

        }

}
}
