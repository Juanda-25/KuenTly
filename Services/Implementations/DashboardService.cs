using KuenTly.Enums;
using KuenTly.Models;
using KuenTly.Services.Interfaces;

namespace KuenTly.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IVentaService _ventaService;
        private readonly IRecordatorioService _recordatorioService;

        public DashboardService(IVentaService ventaService, IRecordatorioService recordatorioService)
        {
            _ventaService = ventaService;
            _recordatorioService = recordatorioService;
        }

        public async Task<DashboardResumen> ObtenerResumenAsync()
        {
            var ventas = await _ventaService.ObtenerTodasActivasAsync();
            var pendientes = ventas.Where(v => v.Estado != EstadoCuenta.Cancelada).ToList();

            var saldoTotal = pendientes.Sum(v => v.Saldo);

            var clientesEnMora = pendientes
                .Where(v => v.Estado == EstadoCuenta.EnMora)
                .Select(v => v.Venta.ClienteId)
                .Distinct()
                .Count();

            var ventasAlDia = pendientes.Count(v => v.Estado == EstadoCuenta.AlDia);
            var ventasEnMora = pendientes.Count(v => v.Estado == EstadoCuenta.EnMora);

            var recordatorios = await _recordatorioService.ObtenerAsync();
            var proximos = recordatorios.OrderBy(r => r.Fecha).Take(5).ToList();

            return new DashboardResumen
            {
                SaldoTotalPendiente = saldoTotal,
                ClientesEnMora = clientesEnMora,
                VentasAlDia = ventasAlDia,
                VentasEnMora = ventasEnMora,
                ProximosRecordatorios = proximos
            };
        }
    }
}