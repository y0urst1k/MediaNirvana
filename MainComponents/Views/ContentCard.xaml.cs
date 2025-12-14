using System.Windows;
using System.Windows.Controls;
using Infrastructure.EF.Entity.IndependentEntity;

namespace MainComponents.Views
{
    public partial class ContentCard : UserControl
    {
        // === 1. Dependency Property (Входные данные) ===
        // Это аналог props.item в React
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(MediaItem), typeof(ContentCard));

        public MediaItem Item
        {
            get { return (MediaItem)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        // === 2. События (Аналог props.onDelete, props.onUpdate) ===
        // В WPF события обычно "пузырятся" (Bubbling), но мы сделаем прямые C# события

        // Определяем делегаты событий
        public delegate void CardActionHandler(object sender, MediaItem item);
        public event CardActionHandler ViewDetailClicked;
        public event CardActionHandler DeleteClicked;
        public event CardActionHandler EditClicked; // Для обновления

        public ContentCard()
        {
            InitializeComponent();
        }

        // === 3. Обработчики кнопок ===

        private void OnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            ViewDetailClicked?.Invoke(this, Item);
        }

        private void OnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Можно добавить MessageBox с подтверждением здесь
            if (MessageBox.Show($"Delete '{Item.Title}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteClicked?.Invoke(this, Item);
            }
        }

        private void OnEdit_Click(object sender, RoutedEventArgs e)
        {
            // 1. Создаем окно редактирования и передаем текущий элемент
            var editDialog = new EditContentDialog(Item);

            // 2. Находим родительское окно, чтобы диалог был модальным
            editDialog.Owner = Window.GetWindow(this);

            // 3. Показываем диалог
            if (editDialog.ShowDialog() == true)
            {
                // 4. Если нажали Save, получаем обновленный элемент
                var updatedItem = editDialog.UpdatedItem;

                // 5. ВАЖНО: Нам нужно обновить данные в ObservableCollection.
                // Так как ContentCard не имеет доступа к коллекции, мы кидаем событие наверх.
                // Но для простоты MVVM без фреймворков, мы можем обновить свойства ТЕКУЩЕГО объекта Item
                // (так как Item - это ссылочный тип, изменения отразятся везде)

                UpdateCurrentItem(Item, updatedItem);

                // Или кидаем событие, если нужна перерисовка списка
                EditClicked?.Invoke(this, Item);
            }
        }

        // Метод для копирования свойств из нового объекта в старый
        private void UpdateCurrentItem(MediaItem target, MediaItem source)
        {
            target.Title = source.Title;
            target.OriginalTitle = source.OriginalTitle;
            target.Type = source.Type;
            target.Status = source.Status;
            target.Year = source.Year;
            target.Country = source.Country;
            target.DurationMinutes = source.DurationMinutes;
            target.SeasonsCount = source.SeasonsCount;
            target.EpisodesCount = source.EpisodesCount;
            target.OfficialRating = source.OfficialRating;
            target.Synopsis = source.Synopsis;
            target.Tags = source.Tags;
            target.Notes = source.Notes;
            target.UpdatedDate = source.UpdatedDate;

            // Благодаря INotifyPropertyChanged в классе ContentItem, UI обновится сам
        }
    }
}