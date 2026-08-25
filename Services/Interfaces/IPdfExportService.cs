using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IPdfExportService
    {
        Task<string> ExportarVentasAsync(List<VentaResumen> ventas, string tituloReporte);
    }
}