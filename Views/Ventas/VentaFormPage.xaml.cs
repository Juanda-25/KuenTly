using KuenTly.ViewModels.Ventas;

namespace KuenTly.Views.Ventas
{
    public partial class VentaFormPage : ContentPage
    {
        public VentaFormPage(VentaFormViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}