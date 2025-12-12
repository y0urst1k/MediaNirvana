using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MediaTracker.Views
{
    public partial class MediaDetailView : UserControl, INotifyPropertyChanged
    {
        // 1. Dependency Property: ITEM (Исходный объект)
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(ContentItem), typeof(MediaDetailView),
                new PropertyMetadata(null, OnItemChanged));

        public ContentItem Item
        {
            get => (ContentItem)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        // 2. Dependency Property: ALL ITEMS (Для поиска похожих)
        public static readonly DependencyProperty AllItemsProperty =
            DependencyProperty.Register("AllItems", typeof(ObservableCollection<ContentItem>), typeof(MediaDetailView));

        public ObservableCollection<ContentItem> AllItems
        {
            get => (ObservableCollection<ContentItem>)GetValue(AllItemsProperty);
            set => SetValue(AllItemsProperty, value);
        }

        // 3. Локальные свойства для UI
        private ContentItem _tempItem;
        public ContentItem TempItem // Копия для редактирования
        {
            get => _tempItem;
            set { _tempItem = value; OnPropertyChanged(); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ContentItem> RelatedItems { get; set; } = new ObservableCollection<ContentItem>();

        // 4. События
        public event EventHandler BackClicked;
        public event EventHandler<ContentItem> DeleteClicked;
        // Можно добавить UpdateClicked, если нужно сохранять в БД

        public MediaDetailView()
        {
            InitializeComponent();
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as MediaDetailView;
            if (view != null && e.NewValue is ContentItem newItem)
            {
                view.LoadItem(newItem);
            }
        }

        private void LoadItem(ContentItem item)
        {
            // Создаем копию для отображения/редактирования
            TempItem = CloneItem(item);
            IsEditing = false;
            FindRelatedItems(item);

            // Устанавливаем статус в комбобоксе
            foreach (ComboBoxItem cbItem in StatusCombo.Items)
            {
                if (cbItem.Content.ToString() == item.Status.ToString())
                    StatusCombo.SelectedItem = cbItem;
            }
        }

        // Логика "Похожих элементов" из React
        private void FindRelatedItems(ContentItem current)
        {
            RelatedItems.Clear();
            if (AllItems == null) return;

            string firstWord = current.Title.Split(' ')[0];

            var related = AllItems
                .Where(i => i.Id != current.Id && (i.Title.Contains(firstWord) || current.Title.Contains(i.Title.Split(' ')[0])))
                .Take(3)
                .ToList();

            foreach (var item in related) RelatedItems.Add(item);
        }

        // === BUTTON HANDLERS ===

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Отменяем изменения: перезагружаем из оригинала
            LoadItem(Item);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем изменения: копируем из Temp в Original
            // Обновляем статус из ComboBox вручную
            string statusStr = (StatusCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (Enum.TryParse(statusStr, out InteractionStatus status))
            {
                TempItem.Status = status;
            }

            // Копируем свойства обратно в Item (ссылка на объект в ObservableCollection)
            ApplyChanges(Item, TempItem);

            IsEditing = false;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Delete '{Item.Title}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteClicked?.Invoke(this, Item);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            BackClicked?.Invoke(this, EventArgs.Empty);
        }

        // === HELPERS ===

        private ContentItem CloneItem(ContentItem source)
        {
            return new ContentItem
            {
                Id = source.Id,
                Title = source.Title,
                OriginalTitle = source.OriginalTitle,
                Type = source.Type,
                Year = source.Year,
                Country = source.Country,
                DurationMinutes = source.DurationMinutes,
                SeasonsCount = source.SeasonsCount,
                EpisodesCount = source.EpisodesCount,
                OfficialRating = source.OfficialRating,
                Synopsis = source.Synopsis,
                Tags = source.Tags, // Reference copy is okay for list display
                Status = source.Status,
                Rating = source.Rating,
                Notes = source.Notes,
                AddedDate = source.AddedDate,
                UpdatedDate = source.UpdatedDate,
                HasPhysicalCopy = source.HasPhysicalCopy,
                HasDigitalCopy = source.HasDigitalCopy,
                Format = source.Format,
                Source = source.Source,
                Location = source.Location,
                PurchasePrice = source.PurchasePrice
            };
        }

        private void ApplyChanges(ContentItem target, ContentItem source)
        {
            target.Status = source.Status;
            target.Rating = source.Rating;
            target.Notes = source.Notes;
            target.HasPhysicalCopy = source.HasPhysicalCopy;
            target.HasDigitalCopy = source.HasDigitalCopy;
            target.Format = source.Format;
            target.Source = source.Source;
            target.Location = source.Location;
            target.PurchasePrice = source.PurchasePrice;
            target.UpdatedDate = DateTime.Now.ToString("d");
            // Остальные поля (Title, Year) в этом View не редактируются по дизайну
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}