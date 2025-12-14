using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dialogs.Views
{
    public partial class EditContentDialog : Window
    {
        // Свойство для возврата обновленных данных
        public ContentItem UpdatedItem { get; private set; }

        // Храним оригинальный ID и даты, чтобы не потерять их
        private string _originalId;
        private string _addedDate;

        public EditContentDialog(ContentItem itemToEdit)
        {
            InitializeComponent();
            PopulateFields(itemToEdit);
        }

        private void PopulateFields(ContentItem item)
        {
            // Сохраняем технические данные
            _originalId = item.Id;
            _addedDate = item.AddedDate;

            // Заполняем поля UI
            TitleInput.Text = item.Title;
            OriginalTitleInput.Text = item.OriginalTitle;
            YearInput.Text = item.Year > 0 ? item.Year.ToString() : "";
            CountryInput.Text = item.Country;

            // ComboBoxes
            SetComboValue(TypeInput, item.Type);
            SetComboValue(StatusInput, item.Status.ToString());

            // Numbers
            DurationInput.Text = item.DurationMinutes?.ToString() ?? "";
            SeasonsInput.Text = item.SeasonsCount?.ToString() ?? "";
            EpisodesInput.Text = item.EpisodesCount?.ToString() ?? "";

            OfficialRatingInput.Text = item.OfficialRating;
            SynopsisInput.Text = item.Synopsis;
            NotesInput.Text = item.Notes;

            // Tags (List<string> -> "tag1, tag2")
            TagsInput.Text = item.Tags != null ? string.Join(", ", item.Tags) : "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleInput.Text))
            {
                MessageBox.Show("Title is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Парсинг тегов
            var tagsList = TagsInput.Text
                .Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            // Парсинг Enum и чисел
            Enum.TryParse(GetComboValue(StatusInput), out InteractionStatus status);
            int.TryParse(YearInput.Text, out int year);
            int.TryParse(DurationInput.Text, out int duration);
            int.TryParse(SeasonsInput.Text, out int seasons);
            int.TryParse(EpisodesInput.Text, out int episodes);

            // Создаем ОБНОВЛЕННЫЙ объект
            // Мы создаем новый инстанс, чтобы соблюдать иммутабельность, 
            // или можно обновлять существующий, если архитектура позволяет.
            UpdatedItem = new ContentItem
            {
                Id = _originalId,
                AddedDate = _addedDate,
                UpdatedDate = DateTime.Now.ToString("d"), // Обновляем дату редактирования

                Title = TitleInput.Text,
                OriginalTitle = OriginalTitleInput.Text,
                Type = GetComboValue(TypeInput),
                Status = status,
                Year = year,
                Country = CountryInput.Text,

                DurationMinutes = duration > 0 ? duration : null,
                SeasonsCount = seasons > 0 ? seasons : null,
                EpisodesCount = episodes > 0 ? episodes : null,

                OfficialRating = OfficialRatingInput.Text,
                Synopsis = SynopsisInput.Text,
                Notes = NotesInput.Text,
                Tags = tagsList
            };

            DialogResult = true; // Закрывает окно и возвращает true в MainWindow
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // --- Вспомогательные методы ---

        private void TypeInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoFields == null) return;

            string type = GetComboValue(TypeInput);

            // Видимость полей как в React
            if (type == "Movie" || type == "Show")
            {
                VideoFields.Visibility = Visibility.Visible;

                // Вложенная логика для Show
                Visibility showVis = (type == "Show") ? Visibility.Visible : Visibility.Collapsed;
                ShowSeasonsField.Visibility = showVis;
                ShowEpisodesField.Visibility = showVis;
            }
            else
            {
                VideoFields.Visibility = Visibility.Collapsed;
            }
        }

        private void SetComboValue(ComboBox box, string value)
        {
            foreach (ComboBoxItem item in box.Items)
            {
                if (item.Content.ToString() == value)
                {
                    box.SelectedItem = item;
                    break;
                }
            }
        }

        private string GetComboValue(ComboBox box)
        {
            return (box.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}