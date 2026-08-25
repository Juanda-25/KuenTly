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
        private readonly IPdfExportService _pdfExportService;
        private readonly IExcelExportService _excelExportService;

        public ReportesViewModel(IReporteService reporteService, IPdfExportService pdfExportService, IExcelExportService excelExportService)
        {
            _reporteService = reporteService;
            _pdfExportService = pdfExportService;
            _excelExportService = excelExportService;
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

        [RelayCommand]
        private async Task ExportarPdfAsync()
        {
            if (Resultados.Count == 0)
            {
                MensajeError = "No hay ventas para exportar con estos filtros.";
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                var titulo = ConstruirTituloReporte();
                var ruta = await _pdfExportService.ExportarVentasAsync(Resultados.ToList(), titulo);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir reporte de ventas",
                    File = new ShareFile(ruta)
                });
            });
        }

        [RelayCommand]
        private async Task ExportarExcelAsync()
        {
            if (Resultados.Count == 0)
            {
                MensajeError = "No hay ventas para exportar con estos filtros.";
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                var ruta = await _excelExportService.ExportarVentasAsync(Resultados.ToList());

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir reporte de ventas",
                    File = new ShareFile(ruta)
                });
            });
        }

        private string ConstruirTituloReporte()
        {
            var partes = new List<string>();
            if (ZonaSeleccionada != "Todas") partes.Add($"Zona: {ZonaSeleccionada}");
            if (EstadoSeleccionado != "Todos") partes.Add($"Estado: {EstadoSeleccionado}");
            return partes.Count > 0 ? $"Reporte de ventas ({string.Join(", ", partes)})" : "Reporte de ventas (todas)";
        }
    }
}