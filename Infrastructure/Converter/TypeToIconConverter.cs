using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Converter
{
    public class TypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string) switch
            {
                "Movie" => "🎬",   // Film
                "Book" => "📖",    // BookOpen
                "Show" => "📺",    // Tv
                "Game" => "🎮",    // Gamepad2
                "Music" => "🎵",   // Music
                "Other" => "📦",   // Package
                _ => "📄"
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}