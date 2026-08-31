using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IVentaService
    {
        Task<List<VentaResumen>> ObtenerPorClienteAsync(int clienteId);

        Task<List<VentaResumen>> ObtenerTodasActivasAsync();

        Task<VentaResumen?> ObtenerResumenAsync(int ventaId);

        Task<Venta?> ObtenerPorIdAsync(int id);

        Task<int> CrearAsync(Venta venta);

        Task ActualizarAsync(Venta venta);

        Task EliminarAsync(int id);
    }
}