using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace KuenTly.ViewModels.Base
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string? MensajeError { get; set; }

        protected async Task EjecutarSeguroAsync(Func<Task> accion)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                MensajeError = null;
                await accion();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ViewModel: {ex}");
                MensajeError = "Ocurrió un error. Intenta de nuevo.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}