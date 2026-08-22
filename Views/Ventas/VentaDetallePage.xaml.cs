using KuenTly.ViewModels.Ventas;

namespace KuenTly.Views.Ventas
{
    public partial class VentaDetallePage : ContentPage
    {
        private readonly VentaDetalleViewModel _viewModel;

        public VentaDetallePage(VentaDetalleViewModel viewModel)
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