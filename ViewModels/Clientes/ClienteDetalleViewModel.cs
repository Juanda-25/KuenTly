using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Clientes
{
    public partial class ClienteDetalleViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IClienteService _clienteService;
        private readonly IVentaService _ventaService;
        private int _clienteId;

        public ClienteDetalleViewModel(IClienteService clienteService, IVentaService ventaService)
        {
            _clienteService = clienteService;
            _ventaService = ventaService;
        }

        [ObservableProperty]
        public partial Cliente? Cliente { get; set; }

        [ObservableProperty]
        public partial VentaResumen? VentaSeleccionada { get; set; }

        public ObservableCollection<VentaResumen> Ventas { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ClienteId", out var valor) && int.TryParse(valor?.ToString(), out var id))
            {
                _clienteId = id;
            }
        }

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                Cliente = await _clienteService.ObtenerPorIdAsync(_clienteId);

                var ventas = await _ventaService.ObtenerPorClienteAsync(_clienteId);
                Ventas.Clear();
                foreach (var venta in ventas)
                    Ventas.Add(venta);
            });
        }

        [RelayCommand]
        private void Llamar()
        {
            if (Cliente is null || string.IsNullOrWhiteSpace(Cliente.Telefono))
                return;

            if (!PhoneDialer.Default.IsSupported)
            {
                MensajeError = "Este dispositivo (o emulador) no admite hacer llamadas.";
                return;
            }

            try
            {
                PhoneDialer.Default.Open(Cliente.Telefono);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo abrir el marcador telefónico.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        [RelayCommand]
        private async Task EditarAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(Views.Clientes.ClienteFormPage)}?ClienteId={_clienteId}");
        }

        [RelayCommand]
        private async Task NuevaVentaAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(Views.Ventas.VentaFormPage)}?ClienteId={_clienteId}");
        }

        partial void OnVentaSeleccionadaChanged(VentaResumen? value)
        {
            if (value is null)
                return;

            var id = value.Venta.Id;
            VentaSeleccionada = null;
            _ = Shell.Current.GoToAsync($"{nameof(Views.Ventas.VentaDetallePage)}?VentaId={id}");
        }
    }
}