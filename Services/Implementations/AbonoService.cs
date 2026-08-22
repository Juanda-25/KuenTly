using KuenTly.Data;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuenTly.Services.Implementations
{
    public class AbonoService : IAbonoService
    {
        private readonly IDbContextFactory<KuenTlyDbContext> _contextFactory;
        private readonly IVentaService _ventaService;

        public AbonoService(IDbContextFactory<KuenTlyDbContext> contextFactory, IVentaService ventaService)
        {
            _contextFactory = contextFactory;
            _ventaService = ventaService;
        }

        public async Task<List<Abono>> ObtenerPorVentaAsync(int ventaId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Abonos
                .Where(a => a.VentaId == ventaId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task CrearAsync(Abono abono)
        {
            var resumen = await _ventaService.ObtenerResumenAsync(abono.VentaId);
            if (resumen is null)
                throw new InvalidOperationException("La venta a la que pertenece este abono no existe.");

            if (abono.Valor > resumen.Saldo)
                throw new InvalidOperationException($"El abono supera el saldo pendiente de $ {resumen.Saldo:N0}.");

            using var context = await _contextFactory.CreateDbContextAsync();
            context.Abonos.Add(abono);
            await context.SaveChangesAsync();
        }

        public async Task AnularAsync(int abonoId, string motivo)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var abono = await context.Abonos.FindAsync(abonoId);
            if (abono is null)
                return;

            abono.Anulado = true;
            abono.FechaAnulacion = DateTime.Now;
            abono.MotivoAnulacion = motivo;
            await context.SaveChangesAsync();
        }
    }
}