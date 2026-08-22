using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Enums;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Abonos
{
    public partial class AbonoFormViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IAbonoService _abonoService;
        private int _ventaId;

        public AbonoFormViewModel(IAbonoService abonoService)
        {
            _abonoService = abonoService;
        }

        public ObservableCollection<MetodoPago> MetodosPago { get; } = new(Enum.GetValues<MetodoPago>());

        [ObservableProperty]
        public partial DateTime Fecha { get; set; } = DateTime.Now;

        [ObservableProperty]
        public partial decimal Valor { get; set; }

        [ObservableProperty]
        public partial MetodoPago MetodoPago { get; set; } = MetodoPago.Efectivo;

        [ObservableProperty]
        public partial string? Referencia { get; set; }

        [ObservableProperty]
        public partial string? Observaciones { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("VentaId", out var valor) && int.TryParse(valor?.ToString(), out var id))
            {
                _ventaId = id;
            }
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (Valor <= 0)
            {
                MensajeError = "El valor del abono debe ser mayor a cero.";
                return;
            }

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                MensajeError = null;

                var abono = new Abono
                {
                    VentaId = _ventaId,
                    Fecha = Fecha,
                    Valor = Valor,
                    MetodoPago = MetodoPago,
                    Referencia = Referencia,
                    Observaciones = Observaciones
                };

                await _abonoService.CrearAsync(abono);

                await Shell.Current.GoToAsync("..");
            }
            catch (InvalidOperationException ex)
            {
                // Único lugar de la app donde mostramos el mensaje real de la excepción:
                // la persona necesita saber exactamente cuánto puede abonar.
                MensajeError = ex.Message;
            }
            catch (Exception ex)
            {
                MensajeError = "Ocurrió un error. Intenta de nuevo.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}