using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MediaTracker
{
    // --- MODELS ---
    public enum InteractionStatus { Planned, Watching, Reading, Completed, Abandoned, Paused }

    public class ContentItem : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Type { get; set; } // Movie, Book, Show, Game, etc.
        public int Year { get; set; }
        public string Country { get; set; }

        // Детали
        public int? DurationMinutes { get; set; }
        public int? SeasonsCount { get; set; }
        public int? EpisodesCount { get; set; }
        public string OfficialRating { get; set; }
        public string Synopsis { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        // Вспомогательное свойство для отображения тегов в UI
        public string TagsString => Tags != null && Tags.Any() ? string.Join(", ", Tags) : "";

        // Статус и рейтинг (с уведомлением UI)
        private InteractionStatus _status;
        public InteractionStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        private double? _rating;
        public double? Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(); }
        }

        public string AddedDate { get; set; }
        public string UpdatedDate { get; set; } = DateTime.Now.ToString("d");

        // Инвентарь
        public bool HasPhysicalCopy { get; set; }
        public bool HasDigitalCopy { get; set; }
        public string Format { get; set; }
        public string Source { get; set; }
        public string Location { get; set; }
        public double? PurchasePrice { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class PersonalList : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "Watchlist", "Favorites", "Custom"

        // Список ID элементов, которые лежат в этом списке
        public List<string> ItemIds { get; set; } = new List<string>();

        // Boilerplate для обновления UI
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- MAIN LOGIC ---
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ContentItem> Items { get; set; }
        public ObservableCollection<PersonalList> Lists { get; set; }

        private string _currentUsername;
        public string CurrentUsername
        {
            get => _currentUsername;
            set { _currentUsername = value; OnPropertyChanged(); }
        }

        private ContentItem _selectedItem;
        public ContentItem SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        // Для выпадающего списка в деталях
        public Array StatusOptions => Enum.GetValues(typeof(InteractionStatus));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Инициализация тестовых данных
            Items = new ObservableCollection<ContentItem>
            {
                new ContentItem
                {
                    Id="1", Title="The Great Gatsby", OriginalTitle="The Great Gatsby", Type="Book",
                    Status=InteractionStatus.Reading, Notes="Really enjoying the prose", Year=1925,
                    Synopsis="A story of the fabulously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.",
                    Tags=new List<string>{"Classic", "Drama"}
                },
                new ContentItem
                {
                    Id="2", Title="Inception", Type="Movie", Status=InteractionStatus.Completed, Year=2010, Rating=9.0,
                    Country="USA", OfficialRating="PG-13", DurationMinutes=148,
                    Tags=new List<string>{"Sci-Fi", "Action"}
                },
                new ContentItem
                {
                    Id="3", Title="Breaking Bad", Type="Show", Status=InteractionStatus.Watching, Notes="Season 3, Ep 8", Year=2008,
                    SeasonsCount=5, EpisodesCount=62
                },
                new ContentItem { Id="4", Title="Dune", Type="Book", Status=InteractionStatus.Planned, Year=1965 },
                new ContentItem { Id="5", Title="The Witcher 3", Type="Game", Status=InteractionStatus.Paused, Notes="Stuck on boss", Year=2015 },
                new ContentItem { Id="6", Title="Abbey Road", Type="Music", Status=InteractionStatus.Completed, Year=1969 }
            };

            Lists = new ObservableCollection<PersonalList>
            {
                new PersonalList { Id="1", Name="Favorites 2023", Type="Favorites", Description="Best content of the year" },
                new PersonalList { Id="2", Name="Date Night Movies", Type="Watchlist", Description="To watch with girlfriend" }
            };
        }

        // --- HANDLERS ---

        // Логин
        private void OnLoginSuccess(object sender, string username)
        {
            // 1. Устанавливаем пользователя
            CurrentUsername = username;

            // 2. Скрываем экран логина
            LoginView.Visibility = Visibility.Collapsed;

            // 3. Показываем основной интерфейс
            MainAppView.Visibility = Visibility.Visible;

            // 4. По умолчанию открываем библиотеку
            SwitchScreen("Library");
        }
        // Навигация
        private void NavToLibrary_Click(object sender, RoutedEventArgs e) => SwitchScreen("Library");
        private void NavToLists_Click(object sender, RoutedEventArgs e) => SwitchScreen("Lists");

        // Обработка клика в сайдбаре (список справа)
        private void SidebarItem_Click(object sender, ContentItem item) // Обратите внимание: сигнатура изменилась
        {
            // UserControl Sidebar кидает сразу объект ContentItem, а не MouseButtonEventArgs
            SelectedItem = item;
            SwitchScreen("Detail");
        }

        // Обработчик события Navigate из Sidebar
        private void OnSidebarNavigate(object sender, string screen)
        {
            if (screen == "library") SwitchScreen("Library");
            else if (screen == "lists") SwitchScreen("Lists");
        }

        // Кнопка "+ Add Item"
        // Это событие приходит из ContentTrackerView при нажатии кнопки "+"
        private void ContentTracker_AddRequested(object sender, RoutedEventArgs e)
        {
            // Вызываем ту же логику добавления
            var dialog = new MediaTracker.Views.AddContentDialog();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                var newItem = dialog.CreatedItem;
                Items.Insert(0, newItem);

                // ВАЖНО: При использовании ObservableCollection с CollectionViewSource
                // иногда нужно пнуть UI, но обычно Insert работает сразу.
            }
        }

        // Событие от ContentCard: Удаление
        // (Сигнатура должна совпадать с делегатом в ContentCard.cs)
        private void Card_Delete(object sender, ContentItem item)
        {
            if (MessageBox.Show($"Are you sure you want to delete '{item.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Items.Remove(item);
                // Если удалили то, что сейчас открыто в деталях - закрыть детали
                if (SelectedItem == item)
                {
                    SwitchScreen("Library");
                }
            }
        }

        // Событие от ContentCard: Просмотр деталей
        private void Card_ViewDetail(object sender, ContentItem item)
        {
            ShowDetail(item);
        }

        // Кнопка "Back" в деталях
        private void BackToLibrary_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = null;
            SwitchScreen("Library");
        }

        // Кнопка "Delete" внутри экрана деталей
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                Card_Delete(this, SelectedItem);
            }
        }

        // Вспомогательный метод для открытия деталей
        private void ShowDetail(ContentItem item)
        {
            SelectedItem = item;
            SwitchScreen("Detail");
        }

        // Переключение видимости экранов
        private void SwitchScreen(string screenName)
        {
            LibraryScreen.Visibility = Visibility.Collapsed;
            ListsScreen.Visibility = Visibility.Collapsed;
            DetailScreen.Visibility = Visibility.Collapsed;

            switch (screenName)
            {
                case "Library": LibraryScreen.Visibility = Visibility.Visible; break;
                case "Lists": ListsScreen.Visibility = Visibility.Visible; break;
                case "Detail": DetailScreen.Visibility = Visibility.Visible; break;
            }
        }

        // Boilerplate для INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}