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
        private int _ventaId;
        private decimal _totalAbonadoActual;

        public VentaFormViewModel(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [ObservableProperty]
        public partial string Titulo { get; set; } = "Nueva venta";

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
            if (query.TryGetValue("ClienteId", out var clienteValor) && int.TryParse(clienteValor?.ToString(), out var clienteId))
            {
                _clienteId = clienteId;
            }

            if (query.TryGetValue("VentaId", out var ventaValor) && int.TryParse(ventaValor?.ToString(), out var ventaId))
            {
                _ventaId = ventaId;
                _ = CargarAsync();
            }
        }

        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                var venta = await _ventaService.ObtenerPorIdAsync(_ventaId);
                if (venta is null)
                    return;

                Titulo = "Editar venta";
                _clienteId = venta.ClienteId;
                FechaVenta = venta.FechaVenta;
                ValorTotal = venta.ValorTotal;
                Descripcion = venta.Descripcion;
                FechaPagoAcordada = venta.FechaPagoAcordada;

                _totalAbonadoActual = venta.Abonos.Where(a => !a.Anulado).Sum(a => a.Valor);
            });
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

            if (_ventaId != 0 && ValorTotal < _totalAbonadoActual)
            {
                MensajeError = $"El valor total no puede ser menor a lo ya abonado ($ {_totalAbonadoActual:N0}).";
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                if (_ventaId == 0)
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
                }
                else
                {
                    var venta = await _ventaService.ObtenerPorIdAsync(_ventaId);
                    if (venta is null)
                        return;

                    venta.FechaVenta = FechaVenta;
                    venta.ValorTotal = ValorTotal;
                    venta.Descripcion = Descripcion.Trim();
                    venta.FechaPagoAcordada = FechaPagoAcordada;

                    await _ventaService.ActualizarAsync(venta);
                }

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