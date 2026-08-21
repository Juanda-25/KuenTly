using KuenTly.Models;

namespace KuenTly.Services.Interfaces
{
    public interface IClienteService
    {
        Task<List<Cliente>> ObtenerTodosAsync(bool incluirInactivos = false);

        Task<Cliente?> ObtenerPorIdAsync(int id);

        Task<List<Cliente>> BuscarAsync(string? textoBusqueda, string? zona = null);

        Task<List<string>> ObtenerZonasAsync();

        Task<int> CrearAsync(Cliente cliente);

        Task ActualizarAsync(Cliente cliente);

        Task EliminarAsync(int id);
    }
}