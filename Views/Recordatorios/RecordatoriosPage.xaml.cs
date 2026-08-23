using KuenTly.ViewModels.Recordatorios;

namespace KuenTly.Views.Recordatorios
{
    public partial class RecordatoriosPage : ContentPage
    {
        private readonly RecordatoriosViewModel _viewModel;

        public RecordatoriosPage(RecordatoriosViewModel viewModel)
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