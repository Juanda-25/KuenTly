using KuenTly.ViewModels.Clientes;

namespace KuenTly.Views.Clientes
{
    public partial class ClienteDetallePage : ContentPage
    {
        private readonly ClienteDetalleViewModel _viewModel;

        public ClienteDetallePage(ClienteDetalleViewModel viewModel)
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