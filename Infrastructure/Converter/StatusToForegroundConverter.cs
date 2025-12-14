using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Infrastructure.Converter
{
    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()) switch
            {
                "Planned" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E40AF")),
                "Watching" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#166534")),
                "Reading" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B21A8")),
                "Completed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#065F46")),
                "Paused" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#854D0E")),
                "Abandoned" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#991B1B")),
                _ => Brushes.Black
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}