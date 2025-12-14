using System.Windows;
using System.Windows.Controls;

namespace Infrastructure.AttachedProperties
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextBoxHelper),
            new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(DependencyObject obj) =>
            (string)obj.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(DependencyObject obj, string value) =>
            obj.SetValue(PlaceholderProperty, value);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null) return;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.TextChanged += TextBox_TextChanged;

            UpdatePlaceholderVisibility(textBox);
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e) =>
            UpdatePlaceholderVisibility((TextBox)sender);

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e) =>
            UpdatePlaceholderVisibility((TextBox)sender);

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            UpdatePlaceholderVisibility((TextBox)sender);

        private static void UpdatePlaceholderVisibility(TextBox textBox)
        {
            var placeholder = GetPlaceholder(textBox);
            textBox.SetValue(TextBox.TagProperty, string.IsNullOrEmpty(textBox.Text) ? placeholder : null);
        }
    }
}
