using KuenTly.ViewModels.Configuracion;

namespace KuenTly.Views.Configuracion
{
    public partial class ConfiguracionPage : ContentPage
    {
        public ConfiguracionPage(ConfiguracionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}