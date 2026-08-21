using KuenTly.ViewModels.Clientes;

namespace KuenTly.Views.Clientes
{
    public partial class ClienteFormPage : ContentPage
    {
        public ClienteFormPage(ClienteFormViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}