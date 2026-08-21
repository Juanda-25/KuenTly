using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;
using System.Collections.ObjectModel;

namespace KuenTly.ViewModels.Clientes
{
    public partial class ClienteFormViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IClienteService _clienteService;
        private int _id;

        public ClienteFormViewModel(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [ObservableProperty]
        public partial string Titulo { get; set; } = "Nuevo cliente";

        [ObservableProperty]
        public partial string Nombre { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Telefono { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string? Direccion { get; set; }

        [ObservableProperty]
        public partial string? Zona { get; set; }

        [ObservableProperty]
        public partial string? Observaciones { get; set; }

        public ObservableCollection<string> ZonasSugeridas { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ClienteId", out var valor) && int.TryParse(valor?.ToString(), out var id))
            {
                _id = id;
            }

            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                var zonas = await _clienteService.ObtenerZonasAsync();
                ZonasSugeridas.Clear();
                foreach (var zona in zonas)
                    ZonasSugeridas.Add(zona);

                if (_id != 0)
                {
                    var cliente = await _clienteService.ObtenerPorIdAsync(_id);
                    if (cliente is not null)
                    {
                        Titulo = "Editar cliente";
                        Nombre = cliente.Nombre;
                        Telefono = cliente.Telefono;
                        Direccion = cliente.Direccion;
                        Zona = cliente.Zona;
                        Observaciones = cliente.Observaciones;
                    }
                }
            });
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Telefono))
            {
                MensajeError = "El nombre y el teléfono son obligatorios.";
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                if (_id == 0)
                {
                    var nuevoCliente = new Cliente
                    {
                        Nombre = Nombre.Trim(),
                        Telefono = Telefono.Trim(),
                        Direccion = Direccion,
                        Zona = Zona,
                        Observaciones = Observaciones
                    };
                    await _clienteService.CrearAsync(nuevoCliente);
                }
                else
                {
                    var cliente = await _clienteService.ObtenerPorIdAsync(_id);
                    if (cliente is null)
                        return;

                    cliente.Nombre = Nombre.Trim();
                    cliente.Telefono = Telefono.Trim();
                    cliente.Direccion = Direccion;
                    cliente.Zona = Zona;
                    cliente.Observaciones = Observaciones;

                    await _clienteService.ActualizarAsync(cliente);
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