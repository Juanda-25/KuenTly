using KuenTly.Models;
using Microsoft.EntityFrameworkCore;

namespace KuenTly.Data
{
    public class KuenTlyDbContext : DbContext
    {
        public KuenTlyDbContext(DbContextOptions<KuenTlyDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<Abono> Abonos => Set<Abono>();
        public DbSet<Recordatorio> Recordatorios => Set<Recordatorio>();
        public DbSet<Configuracion> Configuraciones => Set<Configuracion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----- Cliente -----
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired();
                entity.Property(c => c.Telefono).IsRequired();
                entity.HasIndex(c => c.Telefono);
                entity.HasIndex(c => c.Zona);
            });

            // ----- Venta -----
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Descripcion).IsRequired();
                entity.Property(v => v.ValorTotal).HasPrecision(18, 2);
                entity.HasIndex(v => v.ClienteId);
                entity.HasIndex(v => v.FechaPagoAcordada);

                entity.HasOne(v => v.Cliente)
                      .WithMany(c => c.Ventas)
                      .HasForeignKey(v => v.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Abono -----
            modelBuilder.Entity<Abono>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Valor).HasPrecision(18, 2);
                entity.HasIndex(a => a.VentaId);

                entity.HasOne(a => a.Venta)
                      .WithMany(v => v.Abonos)
                      .HasForeignKey(a => a.VentaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Recordatorio -----
            modelBuilder.Entity<Recordatorio>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Mensaje).IsRequired();
                entity.HasIndex(r => r.Fecha);
                entity.HasIndex(r => r.ClienteId);

                entity.HasOne(r => r.Cliente)
                      .WithMany(c => c.Recordatorios)
                      .HasForeignKey(r => r.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Venta)
                      .WithMany()
                      .HasForeignKey(r => r.VentaId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ----- Configuracion -----
            modelBuilder.Entity<Configuracion>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.NombreNegocio).IsRequired();
            });
        }
    }
}