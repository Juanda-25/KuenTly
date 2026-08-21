using KuenTly.Views.Clientes;

namespace KuenTly
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ClienteFormPage), typeof(ClienteFormPage));
        }
    }
}