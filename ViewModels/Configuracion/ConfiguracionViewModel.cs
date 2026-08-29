using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Configuracion
{
    public partial class ConfiguracionViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private readonly IArchivoGuardadorService _archivoGuardadorService;

        public ConfiguracionViewModel(IBackupService backupService, IArchivoGuardadorService archivoGuardadorService)
        {
            _backupService = backupService;
            _archivoGuardadorService = archivoGuardadorService;
        }

        [ObservableProperty]
        public partial string? MensajeExito { get; set; }

        [RelayCommand]
        private async Task CrearBackupAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                MensajeExito = null;

                var rutaTemporal = await _backupService.CrearBackupAsync();
                var nombreSugerido = Path.GetFileName(rutaTemporal);

                var guardado = await _archivoGuardadorService.GuardarArchivoAsync(rutaTemporal, nombreSugerido);

                if (guardado)
                    MensajeExito = "Copia de seguridad guardada correctamente.";
                else
                    MensajeError = "No se guardó la copia de seguridad (se canceló la selección de ubicación).";
            });
        }

        [RelayCommand]
        private async Task RestaurarBackupAsync()
        {
            var confirmar = await Shell.Current.DisplayAlertAsync(
                "Restaurar copia de seguridad",
                "Esto va a reemplazar todos los datos actuales de la app (clientes, ventas, abonos, recordatorios) con los de la copia que elijas. Esta acción no se puede deshacer fácilmente. ¿Deseas continuar?",
                "Sí, continuar",
                "Cancelar");

            if (!confirmar)
                return;

            FileResult? archivo;
            try
            {
                archivo = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona el archivo de copia de seguridad (.db)"
                });
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo abrir el selector de archivos.";
                System.Diagnostics.Debug.WriteLine(ex);
                return;
            }

            if (archivo is null)
                return;

            await EjecutarSeguroAsync(async () =>
            {
                MensajeExito = null;

                var rutaTemporal = Path.Combine(FileSystem.CacheDirectory, "kuently_restaurar_temp.db");

                using (var streamOrigen = await archivo.OpenReadAsync())
                using (var streamDestino = File.Create(rutaTemporal))
                {
                    await streamOrigen.CopyToAsync(streamDestino);
                }

                var esValido = await _backupService.EsBackupValidoAsync(rutaTemporal);
                if (!esValido)
                {
                    MensajeError = "El archivo seleccionado no es una copia de seguridad válida de KuenTly.";
                    File.Delete(rutaTemporal);
                    return;
                }

                await _backupService.RestaurarBackupAsync(rutaTemporal);
                File.Delete(rutaTemporal);

                MensajeExito = "Restauración completada. Cierra y vuelve a abrir la app para ver los datos restaurados.";
            });
        }
    }
}