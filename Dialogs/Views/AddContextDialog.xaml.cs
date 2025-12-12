using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaTracker.Views
{
    public partial class AddContentDialog : Window
    {
        // Это свойство заберет MainWindow после закрытия диалога
        public ContentItem CreatedItem { get; private set; }

        public AddContentDialog()
        {
            InitializeComponent();
        }

        // --- UI LOGIC ---

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(TitleInput.Text))
            {
                MessageBox.Show("Title is required!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Парсинг чисел
            int.TryParse(YearInput.Text, out int year);
            double.TryParse(RatingInput.Text, out double rating);

            // Получение выбранных значений ComboBox
            string type = (TypeInput.SelectedItem as ComboBoxItem)?.Content.ToString();
            string statusStr = (StatusInput.SelectedItem as ComboBoxItem)?.Content.ToString();
            Enum.TryParse(statusStr, out InteractionStatus status);

            // Создание объекта (Сбор данных)
            CreatedItem = new ContentItem
            {
                Id = DateTime.Now.Ticks.ToString(), // Генерируем ID
                Title = TitleInput.Text,
                // OriginalTitle... (можно добавить остальные поля)
                Type = type,
                Year = year,
                Status = status,
                Notes = NotesInput.Text,
                Rating = rating > 0 ? rating : null,
                AddedDate = DateTime.Now.ToString("yyyy-MM-dd")
            };

            // Закрываем окно с результатом "Успех"
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // --- UI EVENTS (Скрытие/Показ полей) ---

        private void TypeInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoFields == null || ShowFields == null) return; // Защита от null при инициализации

            var selectedType = (TypeInput.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Логика видимости как в React {type === 'Movie' || type === 'Show'}
            if (selectedType == "Movie" || selectedType == "Show")
            {
                VideoFields.Visibility = Visibility.Visible;
            }
            else
            {
                VideoFields.Visibility = Visibility.Collapsed;
            }

            // Только для сериалов
            ShowFields.Visibility = selectedType == "Show" ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Inventory_Checked(object sender, RoutedEventArgs e)
        {
            if (InventoryDetails == null) return;

            bool hasPhysical = PhysicalCopyCheck.IsChecked == true;
            bool hasDigital = DigitalCopyCheck.IsChecked == true;

            InventoryDetails.Visibility = (hasPhysical || hasDigital)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Разрешить ввод только цифр
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

