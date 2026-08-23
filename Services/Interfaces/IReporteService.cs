using KuenTly.Enums;
using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IReporteService
    {
        Task<List<VentaResumen>> ObtenerFiltradasAsync(string? zona = null, EstadoCuenta? estado = null);

        Task<List<string>> ObtenerZonasAsync();
    }
}