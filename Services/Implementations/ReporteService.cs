using KuenTly.Enums;
using KuenTly.Models;
using KuenTly.Services.Interfaces;

namespace KuenTly.Services.Implementations
{
    public class ReporteService : IReporteService
    {
        private readonly IVentaService _ventaService;
        private readonly IClienteService _clienteService;

        public ReporteService(IVentaService ventaService, IClienteService clienteService)
        {
            _ventaService = ventaService;
            _clienteService = clienteService;
        }

        public async Task<List<VentaResumen>> ObtenerFiltradasAsync(string? zona = null, EstadoCuenta? estado = null)
        {
            var ventas = await _ventaService.ObtenerTodasActivasAsync();

            var query = ventas.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(v => v.Venta.Cliente?.Zona == zona);

            if (estado.HasValue)
                query = query.Where(v => v.Estado == estado.Value);

            return query.OrderBy(v => v.Venta.Cliente?.Nombre).ToList();
        }

        public async Task<List<string>> ObtenerZonasAsync()
        {
            return await _clienteService.ObtenerZonasAsync();
        }
    }
}