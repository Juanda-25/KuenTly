using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IRecordatorioService
    {
        Task<List<Recordatorio>> ObtenerAsync(bool incluirCompletados = false);

        Task<Recordatorio?> ObtenerPorIdAsync(int id);

        Task<int> CrearAsync(Recordatorio recordatorio);

        Task ActualizarAsync(Recordatorio recordatorio);

        Task MarcarCompletadoAsync(int id, bool completado);

        Task EliminarAsync(int id);
    }
}