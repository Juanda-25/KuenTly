using System.Globalization;
using KuenTly.Enums;

namespace KuenTly.Helpers
{
    public class EstadoCuentaToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var clave = value switch
            {
                EstadoCuenta.AlDia => "EstadoAlDia",
                EstadoCuenta.EnMora => "EstadoMora",
                EstadoCuenta.Cancelada => "EstadoCancelada",
                _ => "TextoSecundario"
            };

            return Application.Current?.Resources[clave] ?? Colors.Black;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}