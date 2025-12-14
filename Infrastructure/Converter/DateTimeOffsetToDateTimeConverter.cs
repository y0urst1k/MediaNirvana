using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Converter
{
    public class DateTimeOffsetToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
                return null;
            if (value is DateTimeOffset dto) 
                return dto.DateTime;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
                return null;
            if (!(value is DateTime dt)) 
                return null;
            var offset = TimeZoneInfo.Local.GetUtcOffset(dt);
            return new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), offset);
        }
    }
}
