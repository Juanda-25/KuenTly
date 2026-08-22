using KuenTly.ViewModels.Abonos;

namespace KuenTly.Views.Abonos
{
    public partial class AbonoFormPage : ContentPage
    {
        public AbonoFormPage(AbonoFormViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}