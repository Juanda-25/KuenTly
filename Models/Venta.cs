namespace KuenTly.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        public decimal ValorTotal { get; set; }

        public DateTime FechaPagoAcordada { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        // Relaciones
        public Cliente? Cliente { get; set; }

        public ICollection<Abono> Abonos { get; set; } = new List<Abono>();
    }
}