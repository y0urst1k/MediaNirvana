using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Infrastructure.Converter
{
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, является ли значение целым числом
            if (value is int number)
            {
                // Если 0 - показываем элемент
                if (number == 0) return Visibility.Visible;
            }

            // Во всех остальных случаях скрываем
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
