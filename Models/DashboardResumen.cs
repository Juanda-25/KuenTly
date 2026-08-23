namespace KuenTly.Models
{
    // No es una entidad de base de datos: agrega estadísticas generales del negocio
    // para el Dashboard, reutilizando el cálculo de saldo/estado que ya vive en VentaService.
    public class DashboardResumen
    {
        public decimal SaldoTotalPendiente { get; set; }
        public int ClientesEnMora { get; set; }
        public int VentasAlDia { get; set; }
        public int VentasEnMora { get; set; }
        public List<Recordatorio> ProximosRecordatorios { get; set; } = new();
    }
}