using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Infrastructure.Converter
{
    public class EmptyCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 1. Проверка на null
            if (value == null) return Visibility.Collapsed;

            // 2. Специальная проверка для ICollectionView (фильтрация)
            if (value is ICollectionView view)
            {
                return view.IsEmpty ? Visibility.Collapsed : Visibility.Visible;
            }

            // 3. Проверка для ICollection (List, ObservableCollection)
            if (value is ICollection collection)
            {
                return collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            // 4. Fallback: любой перечисляемый тип (медленно, но надежно)
            if (value is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                if (enumerator.MoveNext()) return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}