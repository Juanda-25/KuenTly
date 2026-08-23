using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Recordatorios
{
    public partial class RecordatorioFormViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IRecordatorioService _recordatorioService;
        private readonly IClienteService _clienteService;
        private int _id;

        public RecordatorioFormViewModel(IRecordatorioService recordatorioService, IClienteService clienteService)
        {
            _recordatorioService = recordatorioService;
            _clienteService = clienteService;
        }

        [ObservableProperty]
        public partial string Titulo { get; set; } = "Nuevo recordatorio";

        [ObservableProperty]
        public partial DateTime Fecha { get; set; } = DateTime.Now.AddDays(1);

        [ObservableProperty]
        public partial string Mensaje { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Cliente? ClienteSeleccionado { get; set; }

        public ObservableCollection<Cliente> Clientes { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("RecordatorioId", out var valor) && int.TryParse(valor?.ToString(), out var id))
            {
                _id = id;
            }

            _ = CargarAsync();
        }

        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                var clientes = await _clienteService.ObtenerTodosAsync();
                Clientes.Clear();
                foreach (var cliente in clientes)
                    Clientes.Add(cliente);

                if (_id != 0)
                {
                    var recordatorio = await _recordatorioService.ObtenerPorIdAsync(_id);
                    if (recordatorio is not null)
                    {
                        Titulo = "Editar recordatorio";
                        Fecha = recordatorio.Fecha;
                        Mensaje = recordatorio.Mensaje;
                        ClienteSeleccionado = Clientes.FirstOrDefault(c => c.Id == recordatorio.ClienteId);
                    }
                }
            });
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (ClienteSeleccionado is null)
            {
                MensajeError = "Debes elegir un cliente.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Mensaje))
            {
                MensajeError = "Escribe qué debes recordar.";
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                if (_id == 0)
                {
                    var nuevo = new Recordatorio
                    {
                        ClienteId = ClienteSeleccionado.Id,
                        Fecha = Fecha,
                        Mensaje = Mensaje.Trim()
                    };
                    await _recordatorioService.CrearAsync(nuevo);
                }
                else
                {
                    var recordatorio = await _recordatorioService.ObtenerPorIdAsync(_id);
                    if (recordatorio is null)
                        return;

                    recordatorio.ClienteId = ClienteSeleccionado.Id;
                    recordatorio.Fecha = Fecha;
                    recordatorio.Mensaje = Mensaje.Trim();

                    await _recordatorioService.ActualizarAsync(recordatorio);
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