using KuenTly.Enums;

namespace KuenTly.Models
{
    public class Abono
    {
        public int Id { get; set; }

        public int VentaId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Valor { get; set; }

        public MetodoPago MetodoPago { get; set; }

        public string? Referencia { get; set; }

        public string? Observaciones { get; set; }

        public bool Anulado { get; set; } = false;

        // Relación
        public Venta? Venta { get; set; }
    }
}