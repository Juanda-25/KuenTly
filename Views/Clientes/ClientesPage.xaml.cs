using KuenTly.ViewModels.Clientes;

namespace KuenTly.Views.Clientes
{
    public partial class ClientesPage : ContentPage
    {
        private readonly ClientesViewModel _viewModel;

        public ClientesPage(ClientesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.CargarCommand.Execute(null);
        }
    }
}