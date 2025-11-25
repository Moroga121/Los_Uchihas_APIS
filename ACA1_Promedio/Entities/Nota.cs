namespace ACA1_Promedio.Entities
{
    public class Nota
    {
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public double NotaObtenida { get; set; }
    }
}
