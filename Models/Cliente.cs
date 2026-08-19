namespace KuenTly.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        public string? Zona { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string? Observaciones { get; set; }

        public bool Activo { get; set; } = true;

        // Relaciones
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        public ICollection<Recordatorio> Recordatorios { get; set; } = new List<Recordatorio>();
    }
}