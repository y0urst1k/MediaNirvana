using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Infrastructure.Converter
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если значение null или параметр null — скрываем
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            string currentScreen = value.ToString();
            string targetScreen = parameter.ToString();

            // Сравнение без учета регистра (Library == library)
            if (string.Equals(currentScreen, targetScreen, StringComparison.OrdinalIgnoreCase))
            {
                return Visibility.Visible;
            }

            // ВАЖНО: Если не совпало — СКРЫВАЕМ
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}