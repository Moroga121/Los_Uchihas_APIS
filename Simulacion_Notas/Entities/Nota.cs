namespace Simulacion_Notas.Entities
{
    public class Nota
    {
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string NombreRubro { get; set; } = string.Empty;
        public decimal NotaObtenida { get; set; }
        public int Año { get; set; }
        public string Periodo { get; set; } = string.Empty;
    }
}
