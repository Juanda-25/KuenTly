namespace KuenTly.Models
{
    public class Recordatorio
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public int? VentaId { get; set; }

        public DateTime Fecha { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public bool Completado { get; set; } = false;

        public bool Activo { get; set; } = true;

        // Relaciones
        public Cliente? Cliente { get; set; }

        public Venta? Venta { get; set; }
    }
}