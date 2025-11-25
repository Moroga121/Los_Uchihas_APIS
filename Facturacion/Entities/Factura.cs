using System.Text.Json.Serialization;

namespace Facturacion.Entities
{
    public class Factura
    {
        public long Numero_Factura { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public int MontoBase { get; set; }
        public decimal IVA { get; set; }
        public decimal MontoTotal { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public DateTime Fecha_Creacion { get; set; }
        public string Estado_Factura { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime Fecha_Reversa { get; set; }

    }

    public class ReversarFactura
    {
        public long NumeroFactura { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class Detalle
    {
        [JsonIgnore]
        public long Numero_Factura { get; set; }
        public int ID_Detalle_Factura { get; set; }
        public string Nombre_Curso { get; set; } = string.Empty;
        public decimal MontoBase { get; set; }
        public decimal IVA { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
