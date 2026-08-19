namespace KuenTly.Models
{
    public class Configuracion
    {
        public int Id { get; set; }

        public string NombreNegocio { get; set; } = string.Empty;

        public string Moneda { get; set; } = "COP";

        public string Cultura { get; set; } = "es-CO";
    }
}