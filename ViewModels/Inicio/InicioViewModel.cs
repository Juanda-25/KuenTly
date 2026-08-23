using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Base;

namespace KuenTly.ViewModels.Inicio
{
    public partial class InicioViewModel : BaseViewModel
    {
        private readonly IDashboardService _dashboardService;

        public InicioViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [ObservableProperty]
        public partial decimal SaldoTotalPendiente { get; set; }

        [ObservableProperty]
        public partial int ClientesEnMora { get; set; }

        [ObservableProperty]
        public partial int VentasAlDia { get; set; }

        [ObservableProperty]
        public partial int VentasEnMora { get; set; }

        public ObservableCollection<Recordatorio> ProximosRecordatorios { get; } = new();

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarSeguroAsync(async () =>
            {
                var resumen = await _dashboardService.ObtenerResumenAsync();

                SaldoTotalPendiente = resumen.SaldoTotalPendiente;
                ClientesEnMora = resumen.ClientesEnMora;
                VentasAlDia = resumen.VentasAlDia;
                VentasEnMora = resumen.VentasEnMora;

                ProximosRecordatorios.Clear();
                foreach (var recordatorio in resumen.ProximosRecordatorios)
                    ProximosRecordatorios.Add(recordatorio);
            });
        }
    }
}