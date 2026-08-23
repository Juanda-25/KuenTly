using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Recordatorios
{
    public partial class RecordatoriosViewModel : BaseViewModel
    {
        private readonly IRecordatorioService _recordatorioService;

        public RecordatoriosViewModel(IRecordatorioService recordatorioService)
        {
            _recordatorioService = recordatorioService;
        }

        [ObservableProperty]
        public partial bool MostrarCompletados { get; set; }

        [ObservableProperty]
        public partial Recordatorio? RecordatorioSeleccionado { get; set; }

        public ObservableCollection<Recordatorio> Recordatorios { get; } = new();

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(CargarInternoAsync);
        }

        private async Task CargarInternoAsync()
        {
            var lista = await _recordatorioService.ObtenerAsync(MostrarCompletados);
            Recordatorios.Clear();
            foreach (var recordatorio in lista)
                Recordatorios.Add(recordatorio);
        }

        partial void OnMostrarCompletadosChanged(bool value) => _ = CargarAsync();

        [RelayCommand]
        private async Task NuevoAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.Recordatorios.RecordatorioFormPage));
        }

        [RelayCommand]
        private async Task ToggleCompletadoAsync(Recordatorio? recordatorio)
        {
            if (recordatorio is null)
                return;

            await EjecutarSeguroAsync(async () =>
            {
                await _recordatorioService.MarcarCompletadoAsync(recordatorio.Id, !recordatorio.Completado);
                await CargarInternoAsync();
            });
        }

        partial void OnRecordatorioSeleccionadoChanged(Recordatorio? value)
        {
            if (value is null)
                return;

            var id = value.Id;
            RecordatorioSeleccionado = null;
            _ = Shell.Current.GoToAsync($"{nameof(Views.Recordatorios.RecordatorioFormPage)}?RecordatorioId={id}");
        }
    }
}