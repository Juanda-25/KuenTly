using KuenTly.Data;
using KuenTly.Services.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KuenTly.Services.Implementations
{
    public class BackupService : IBackupService
    {
        private static readonly string[] TablasEsperadas =
        {
            "Clientes", "Ventas", "Abonos", "Recordatorios", "Configuraciones"
        };

        private readonly IDbContextFactory<KuenTlyDbContext> _contextFactory;

        public BackupService(IDbContextFactory<KuenTlyDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string> CrearBackupAsync()
        {
            var rutaBackup = Path.Combine(FileSystem.CacheDirectory, $"KuenTly_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Database.ExecuteSqlRawAsync($"VACUUM INTO '{rutaBackup}'");

            return rutaBackup;
        }

        public async Task<bool> EsBackupValidoAsync(string rutaArchivo)
        {
            try
            {
                await using var connection = new SqliteConnection($"Filename={rutaArchivo}");
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";

                var tablasEncontradas = new HashSet<string>();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tablasEncontradas.Add(reader.GetString(0));
                }

                return TablasEsperadas.All(tabla => tablasEncontradas.Contains(tabla));
            }
            catch
            {
                return false;
            }
        }

        public Task RestaurarBackupAsync(string rutaArchivoValidado)
        {
            var rutaActual = Path.Combine(FileSystem.AppDataDirectory, "kuently.db");
            var rutaSeguridad = Path.Combine(FileSystem.AppDataDirectory, "kuently_antes_de_restaurar.db");

            if (File.Exists(rutaActual))
            {
                File.Copy(rutaActual, rutaSeguridad, overwrite: true);
            }

            File.Copy(rutaArchivoValidado, rutaActual, overwrite: true);

            return Task.CompletedTask;
        }
    }
}