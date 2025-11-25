using MAT02_Matricula.Entities;
using System.Text.Json.Serialization;

namespace MAT02_Matricula.Entities
{
    public class MatriculaCompleta
    {
        public int? Id_matricula { get; set; }
        public string? Curso { get; set; }
        public string? Grupo { get; set; }

        public Expediente_Estudiantes estudiante { get; set; } = null!;
    }
}
