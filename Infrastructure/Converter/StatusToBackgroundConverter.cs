using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Infrastructure.Converter
{
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()) switch
            {
                "Planned" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DBEAFE")), // bg-blue-100
                "Watching" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCFCE7")), // bg-green-100
                "Reading" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3E8FF")), // bg-purple-100
                "Completed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1FAE5")), // bg-emerald-100
                "Paused" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF9C3")), // bg-yellow-100
                "Abandoned" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2")), // bg-red-100
                _ => Brushes.White
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}