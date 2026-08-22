using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Ventas
{
    public partial class VentaDetalleViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IVentaService _ventaService;
        private readonly IAbonoService _abonoService;
        private int _ventaId;

        public VentaDetalleViewModel(IVentaService ventaService, IAbonoService abonoService)
        {
            _ventaService = ventaService;
            _abonoService = abonoService;
        }

        [ObservableProperty]
        public partial VentaResumen? Resumen { get; set; }

        public ObservableCollection<Abono> Abonos { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("VentaId", out var valor) && int.TryParse(valor?.ToString(), out var id))
            {
                _ventaId = id;
            }
        }

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(CargarInternoAsync);
        }

        private async Task CargarInternoAsync()
        {
            Resumen = await _ventaService.ObtenerResumenAsync(_ventaId);

            var abonos = await _abonoService.ObtenerPorVentaAsync(_ventaId);
            Abonos.Clear();
            foreach (var abono in abonos)
                Abonos.Add(abono);
        }

        [RelayCommand]
        private async Task NuevoAbonoAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(Views.Abonos.AbonoFormPage)}?VentaId={_ventaId}");
        }

        [RelayCommand]
        private async Task AnularAbonoAsync(Abono? abono)
        {
            if (abono is null)
                return;

            if (abono.Anulado)
            {
                await Shell.Current.DisplayAlert("Abono anulado", "Este abono ya fue anulado anteriormente.", "Entendido");
                return;
            }

            var motivo = await Shell.Current.DisplayPromptAsync(
                "Anular abono",
                $"Este abono es de $ {abono.Valor:N0} y el saldo pendiente va a aumentar en ese valor. Indica el motivo de la anulación:",
                "Anular",
                "Cancelar",
                placeholder: "Ej: valor registrado por error");

            if (motivo is null)
                return; // Canceló, no hacemos nada.

            if (string.IsNullOrWhiteSpace(motivo))
            {
                await Shell.Current.DisplayAlert("Falta el motivo", "Debes escribir un motivo para anular el abono.", "Entendido");
                return;
            }

            await EjecutarSeguroAsync(async () =>
            {
                await _abonoService.AnularAsync(abono.Id, motivo.Trim());
                await CargarInternoAsync();
            });
        }
    }
}