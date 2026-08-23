using KuenTly.Data;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuenTly.Services.Implementations
{
    public class RecordatorioService : IRecordatorioService
    {
        private readonly IDbContextFactory<KuenTlyDbContext> _contextFactory;

        public RecordatorioService(IDbContextFactory<KuenTlyDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Recordatorio>> ObtenerAsync(bool incluirCompletados = false)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.Recordatorios
                .Include(r => r.Cliente)
                .Where(r => r.Activo);

            if (!incluirCompletados)
                query = query.Where(r => !r.Completado);

            return await query.OrderBy(r => r.Fecha).ToListAsync();
        }

        public async Task<Recordatorio?> ObtenerPorIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Recordatorios.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<int> CrearAsync(Recordatorio recordatorio)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Recordatorios.Add(recordatorio);
            await context.SaveChangesAsync();
            return recordatorio.Id;
        }

        public async Task ActualizarAsync(Recordatorio recordatorio)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Recordatorios.Update(recordatorio);
            await context.SaveChangesAsync();
        }

        public async Task MarcarCompletadoAsync(int id, bool completado)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recordatorio = await context.Recordatorios.FindAsync(id);
            if (recordatorio is null)
                return;

            recordatorio.Completado = completado;
            await context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var recordatorio = await context.Recordatorios.FindAsync(id);
            if (recordatorio is null)
                return;

            recordatorio.Activo = false;
            await context.SaveChangesAsync();
        }
    }
}