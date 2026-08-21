using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Clientes
{
    public partial class ClientesViewModel : BaseViewModel
    {
        private readonly IClienteService _clienteService;

        public ClientesViewModel(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [ObservableProperty]
        public partial string TextoBusqueda { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string? ZonaSeleccionada { get; set; }

        [ObservableProperty]
        public partial Cliente? ClienteSeleccionado { get; set; }

        public ObservableCollection<Cliente> Clientes { get; } = new();

        public ObservableCollection<string> Zonas { get; } = new();

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                var zonas = await _clienteService.ObtenerZonasAsync();
                Zonas.Clear();
                Zonas.Add("Todas");
                foreach (var zona in zonas)
                    Zonas.Add(zona);

                await BuscarInternoAsync();
            });
        }

        [RelayCommand]
        private async Task BuscarAsync()
        {
            await EjecutarSeguroAsync(BuscarInternoAsync);
        }

        private async Task BuscarInternoAsync()
        {
            var zonaFiltro = (string.IsNullOrEmpty(ZonaSeleccionada) || ZonaSeleccionada == "Todas")
                ? null
                : ZonaSeleccionada;

            var resultado = await _clienteService.BuscarAsync(TextoBusqueda, zonaFiltro);

            Clientes.Clear();
            foreach (var cliente in resultado)
                Clientes.Add(cliente);
        }

        [RelayCommand]
        private async Task NuevoClienteAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.Clientes.ClienteFormPage));
        }

        partial void OnTextoBusquedaChanged(string value) => _ = BuscarAsync();

        partial void OnZonaSeleccionadaChanged(string? value) => _ = BuscarAsync();

        partial void OnClienteSeleccionadoChanged(Cliente? value)
        {
            if (value is null)
                return;

            var id = value.Id;
            ClienteSeleccionado = null;
            _ = Shell.Current.GoToAsync($"{nameof(Views.Clientes.ClienteFormPage)}?ClienteId={id}");
        }
    }
}