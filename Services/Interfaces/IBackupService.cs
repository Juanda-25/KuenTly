namespace KuenTly.Services.Interfaces
{
    public interface IBackupService
    {
        Task<string> CrearBackupAsync();

        Task<bool> EsBackupValidoAsync(string rutaArchivo);

        Task RestaurarBackupAsync(string rutaArchivoValidado);
    }
}