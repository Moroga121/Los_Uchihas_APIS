namespace ACA2_Historial.Entities
{
    public class Estudiante_Matriculado
    {
        public string Tipo_Identificacion { get; set; } = string.Empty;
        public string Numero_Identificacion { get; set; } = string.Empty;
        public string Nombre_Completo { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
    }
    public class MatriculaDto
    {
        public int Id_Matricula { get; set; }
        public string Numero_Identificacion { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string Id_Periodo { get; set; } = string.Empty;
    }
}
