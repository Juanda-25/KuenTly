using KuenTly.ViewModels.Reportes;

namespace KuenTly.Views.Reportes
{
    public partial class ReportesPage : ContentPage
    {
        private readonly ReportesViewModel _viewModel;

        public ReportesPage(ReportesViewModel viewModel)
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