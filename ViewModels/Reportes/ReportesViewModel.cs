using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Enums;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Reportes
{
    public partial class ReportesViewModel : BaseViewModel
    {
        private readonly IReporteService _reporteService;

        public ReportesViewModel(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [ObservableProperty]
        public partial string ZonaSeleccionada { get; set; } = "Todas";

        [ObservableProperty]
        public partial string EstadoSeleccionado { get; set; } = "Todos";

        [ObservableProperty]
        public partial VentaResumen? VentaSeleccionada { get; set; }

        [ObservableProperty]
        public partial decimal TotalSaldo { get; set; }

        [ObservableProperty]
        public partial int TotalVentas { get; set; }

        public ObservableCollection<string> Zonas { get; } = new();

        public ObservableCollection<string> Estados { get; } = new() { "Todos", "Al día", "En mora", "Cancelada" };

        public ObservableCollection<VentaResumen> Resultados { get; } = new();

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                var zonas = await _reporteService.ObtenerZonasAsync();
                Zonas.Clear();
                Zonas.Add("Todas");
                foreach (var zona in zonas)
                    Zonas.Add(zona);

                await FiltrarInternoAsync();
            });
        }

        [RelayCommand]
        private async Task FiltrarAsync()
        {
            await EjecutarSeguroAsync(FiltrarInternoAsync);
        }

        private async Task FiltrarInternoAsync()
        {
            string? zonaFiltro = ZonaSeleccionada == "Todas" ? null : ZonaSeleccionada;
            EstadoCuenta? estadoFiltro = EstadoSeleccionado switch
            {
                "Al día" => EstadoCuenta.AlDia,
                "En mora" => EstadoCuenta.EnMora,
                "Cancelada" => EstadoCuenta.Cancelada,
                _ => null
            };

            var resultados = await _reporteService.ObtenerFiltradasAsync(zonaFiltro, estadoFiltro);

            Resultados.Clear();
            foreach (var resultado in resultados)
                Resultados.Add(resultado);

            TotalVentas = resultados.Count;
            TotalSaldo = resultados.Sum(r => r.Saldo);
        }

        partial void OnZonaSeleccionadaChanged(string value) => _ = FiltrarAsync();

        partial void OnEstadoSeleccionadoChanged(string value) => _ = FiltrarAsync();

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