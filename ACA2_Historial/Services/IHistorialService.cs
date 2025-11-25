using ACA2_Historial.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACA2_Historial.Services
{
    public interface IHistorialService
    {
        Task<(bool Exito, string Mensaje, IEnumerable<Estudiante_Matriculado>? Estudiante_Matriculado)> ObtenerEstudiantesMatriculaPeriodoAsync(string Periodo);
        Task<IEnumerable<MatriculaDto>> ObtenerMatriculasAsync();
        Task<(bool, string mensaje)> RegistrarBitacoraAsync(string accion, object descripcion, string accessToken, CancellationToken ct = default);

    }
}
