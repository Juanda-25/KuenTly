using System.Globalization;

namespace KuenTly.Helpers
{
    public class CompletadoToAccionTextoConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var completado = value is bool b && b;
            return completado ? "Reabrir" : "Marcar hecho";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}