using KuenTly.Data;
using KuenTly.Enums;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KuenTly.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly IDbContextFactory<KuenTlyDbContext> _contextFactory;

        public VentaService(IDbContextFactory<KuenTlyDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VentaResumen>> ObtenerPorClienteAsync(int clienteId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var ventas = await context.Ventas
                .Where(v => v.ClienteId == clienteId && v.Activo)
                .Include(v => v.Abonos)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return ventas.Select(CrearResumen).ToList();
        }

        public async Task<VentaResumen?> ObtenerResumenAsync(int ventaId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var venta = await context.Ventas
                .Include(v => v.Abonos)
                .FirstOrDefaultAsync(v => v.Id == ventaId);

            return venta is null ? null : CrearResumen(venta);
        }

        public async Task<Venta?> ObtenerPorIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Ventas
                .Include(v => v.Abonos)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<int> CrearAsync(Venta venta)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Ventas.Add(venta);
            await context.SaveChangesAsync();

            return venta.Id;
        }

        public async Task EliminarAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var venta = await context.Ventas.FindAsync(id);
            if (venta is null)
                return;

            venta.Activo = false;
            await context.SaveChangesAsync();
        }

        // ----- Lógica de negocio centralizada: Saldo y Estado -----
        private static VentaResumen CrearResumen(Venta venta)
        {
            var totalAbonado = venta.Abonos.Where(a => !a.Anulado).Sum(a => a.Valor);
            var saldo = venta.ValorTotal - totalAbonado;

            EstadoCuenta estado;
            if (saldo <= 0)
                estado = EstadoCuenta.Cancelada;
            else if (DateTime.Now.Date <= venta.FechaPagoAcordada.Date)
                estado = EstadoCuenta.AlDia;
            else
                estado = EstadoCuenta.EnMora;

            return new VentaResumen
            {
                Venta = venta,
                Saldo = saldo,
                Estado = estado
            };
        }
    }
}