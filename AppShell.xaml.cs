using KuenTly.Views.Abonos;
using KuenTly.Views.Clientes;
using KuenTly.Views.Ventas;

namespace KuenTly
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ClienteFormPage), typeof(ClienteFormPage));
            Routing.RegisterRoute(nameof(ClienteDetallePage), typeof(ClienteDetallePage));
            Routing.RegisterRoute(nameof(VentaFormPage), typeof(VentaFormPage));
            Routing.RegisterRoute(nameof(VentaDetallePage), typeof(VentaDetallePage));
            Routing.RegisterRoute(nameof(AbonoFormPage), typeof(AbonoFormPage));
        }
    }
}