using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IExcelExportService
    {
        Task<string> ExportarVentasAsync(List<VentaResumen> ventas);
    }
}