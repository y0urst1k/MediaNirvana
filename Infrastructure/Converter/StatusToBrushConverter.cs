using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Infrastructure.Converter
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value?.ToString();
            // Цвета подобраны под ваши классы Tailwind (amber, yellow, gray)
            return status switch
            {
                "Planned" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B45309")), // amber-700
                "Watching" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D97706")), // yellow-600
                "Reading" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EAB308")), // yellow-500
                "Completed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), // green-500 (изменил для контраста)
                "Abandoned" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B5563")), // gray-600
                "Paused" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA580C")), // orange-600
                _ => Brushes.Gray
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}