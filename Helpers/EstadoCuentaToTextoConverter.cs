using System.Globalization;
using KuenTly.Enums;

namespace KuenTly.Helpers
{
    public class EstadoCuentaToTextoConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                EstadoCuenta.AlDia => "Al día",
                EstadoCuenta.EnMora => "En mora",
                EstadoCuenta.Cancelada => "Cancelada",
                _ => string.Empty
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}