using KuenTly.Enums;

namespace KuenTly.Models
{
    // No es una entidad de base de datos: combina una Venta con sus valores calculados
    // (Saldo, Estado) para mostrarlos en pantalla, sin duplicar la lógica de cálculo.
    public class VentaResumen
    {
        public required Venta Venta { get; set; }
        public decimal Saldo { get; set; }
        public EstadoCuenta Estado { get; set; }
    }
}