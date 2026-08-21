using KuenTly.Data;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuenTly.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IDbContextFactory<KuenTlyDbContext> _contextFactory;

        public ClienteService(IDbContextFactory<KuenTlyDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Cliente>> ObtenerTodosAsync(bool incluirInactivos = false)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.Clientes.AsQueryable();

            if (!incluirInactivos)
                query = query.Where(c => c.Activo);

            return await query.OrderBy(c => c.Nombre).ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cliente>> BuscarAsync(string? textoBusqueda, string? zona = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.Clientes.Where(c => c.Activo);

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                var texto = textoBusqueda.Trim();
                query = query.Where(c => c.Nombre.Contains(texto) || c.Telefono.Contains(texto));
            }

            if (!string.IsNullOrWhiteSpace(zona))
            {
                query = query.Where(c => c.Zona == zona);
            }

            return await query.OrderBy(c => c.Nombre).ToListAsync();
        }

        public async Task<List<string>> ObtenerZonasAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Clientes
                .Where(c => c.Activo && c.Zona != null && c.Zona != "")
                .Select(c => c.Zona!)
                .Distinct()
                .OrderBy(z => z)
                .ToListAsync();
        }

        public async Task<int> CrearAsync(Cliente cliente)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            return cliente.Id;
        }

        public async Task ActualizarAsync(Cliente cliente)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Clientes.Update(cliente);
            await context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var cliente = await context.Clientes.FindAsync(id);
            if (cliente is null)
                return;

            cliente.Activo = false;
            await context.SaveChangesAsync();
        }
    }
}