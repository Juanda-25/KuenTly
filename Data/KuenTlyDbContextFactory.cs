using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KuenTly.Data
{
    // Esta clase la usan únicamente las herramientas de línea de comandos de EF Core
    // (dotnet ef migrations add, dotnet ef database update) para crear una instancia
    // del DbContext sin depender del contenedor de inyección de dependencias de la app.
    // No se usa en tiempo de ejecución real de KuenTly.
    public class KuenTlyDbContextFactory : IDesignTimeDbContextFactory<KuenTlyDbContext>
    {
        public KuenTlyDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KuenTlyDbContext>();
            optionsBuilder.UseSqlite("Filename=kuently_design.db");

            return new KuenTlyDbContext(optionsBuilder.Options);
        }
    }
}