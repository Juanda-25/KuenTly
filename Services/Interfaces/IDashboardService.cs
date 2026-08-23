using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumen> ObtenerResumenAsync();
    }
}