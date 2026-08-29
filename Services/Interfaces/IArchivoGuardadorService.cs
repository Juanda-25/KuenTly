namespace KuenTly.Services.Interfaces
{
    public interface IArchivoGuardadorService
    {
        // Devuelve true si el usuario eligió una ubicación y el archivo se guardó ahí.
        // Devuelve false si el usuario canceló el selector sin elegir nada.
        Task<bool> GuardarArchivoAsync(string rutaOrigen, string nombreSugerido);
    }
}