using KuenTly.ViewModels.Recordatorios;

namespace KuenTly.Views.Recordatorios
{
    public partial class RecordatorioFormPage : ContentPage
    {
        public RecordatorioFormPage(RecordatorioFormViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}