using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Ventas
{
    public partial class VentaFormViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IVentaService _ventaService;
        private int _clienteId;

        public VentaFormViewModel(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [ObservableProperty]
        public partial DateTime FechaVenta { get; set; } = DateTime.Now;

        [ObservableProperty]
        public partial decimal ValorTotal { get; set; }

        [ObservableProperty]
        public partial string Descripcion { get; set; } = string.Empty;

        [ObservableProperty]
        public partial DateTime FechaPagoAcordada { get; set; } = DateTime.Now.AddDays(30);

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ClienteId", out var valor) && int.TryParse(valor?.ToString(), out var id))
            {
                _clienteId = id;
            }
        }

        [RelayCommand]
        private void SeleccionarPlazo(string diasTexto)
        {
            if (int.TryParse(diasTexto, out var dias))
            {
                FechaPagoAcordada = FechaVenta.AddDays(dias);
            }
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                MensajeError = "Describe qué compró el cliente.";
                return;
            }

            if (ValorTotal <= 0)
            {
                MensajeError = "El valor total debe ser mayor a cero.";
                return;
            }

            if (FechaPagoAcordada.Date < FechaVenta.Date)
            {
                MensajeError = "La fecha de pago no puede ser anterior a la fecha de venta.";
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                var venta = new Venta
                {
                    ClienteId = _clienteId,
                    FechaVenta = FechaVenta,
                    ValorTotal = ValorTotal,
                    FechaPagoAcordada = FechaPagoAcordada,
                    Descripcion = Descripcion.Trim()
                };

                await _ventaService.CrearAsync(venta);

                await Shell.Current.GoToAsync("..");
            });
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}