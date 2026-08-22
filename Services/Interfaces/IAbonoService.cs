using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IAbonoService
    {
        Task<List<Abono>> ObtenerPorVentaAsync(int ventaId);

        Task CrearAsync(Abono abono);

        Task AnularAsync(int abonoId, string motivo);
    }
}