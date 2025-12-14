using System.Collections.ObjectModel;
using System.Windows.Input;
using Infrastructure.DTO;
using Infrastructure.EF.Enum;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class MediaSidebarViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        // === Свойства для привязки ===
        private ObservableCollection<MediaEditModel> _items;
        public ObservableCollection<MediaEditModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private ObservableCollection<SidebarCategory> _categories;
        public ObservableCollection<SidebarCategory> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        // === Команды ===
        public ICommand NavigateCommand { get; }
        public ICommand CategoryToggleCommand { get; }
        public ICommand ItemClickCommand { get; }


        public MediaSidebarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Categories = new ObservableCollection<SidebarCategory>();

            NavigateCommand = new DelegateCommand<string>(OnNavigate);
            CategoryToggleCommand = new DelegateCommand<SidebarCategory>(OnCategoryToggle);
            ItemClickCommand = new DelegateCommand<MediaEditModel>(OnItemClick);
        }

        // === Логика построения категорий ===
        public void RebuildCategories()
        {
            if (Items == null) return;

            // Определяем типы категорий как MediaType
            var categoryTypes = new[]
            {
                MediaType.Book,
                MediaType.Film,
                MediaType.Series,
                MediaType.Album,
                MediaType.Game,
                MediaType.Other
            };

            // Сохраняем состояние раскрытия (теперь для MediaType)
            var expandedTypes = Categories
                .Where(c => c.IsExpanded)
                .Select(c => c.Type)
                .ToList();

            // По умолчанию раскрыты: Film (Movies), Series (TV Shows), Book (Books)
            if (!expandedTypes.Any())
                expandedTypes.AddRange(new[] { MediaType.Film, MediaType.Series, MediaType.Book });


            Categories.Clear();

            foreach (var type in categoryTypes)
            {
                var itemsInCat = Items
                    .Where(i => i.Type == type)  // Прямое сравнение MediaType
                    .ToList();

                var cat = new SidebarCategory
                {
                    Type = type,
                    Label = GetLabelForType(type),  // Нужно обновить этот метод
                    AllItems = itemsInCat,
                    IsExpanded = expandedTypes.Contains(type)
                };
                Categories.Add(cat);
            }
        }

        private string GetLabelForType(MediaType type)
        {
            return type switch
            {
                MediaType.Book => "Books",
                MediaType.Film => "Movies",
                MediaType.Series => "TV Shows",
                MediaType.Album => "Music",
                MediaType.Game => "Games",
                _ => "Other"
            };
        }

        // === Обработчики команд ===
        private void OnNavigate(string screen)
        {
            switch (screen)
            {
                case "Library":
                    _eventAggregator.GetEvent<NavigateToEvent>().Publish("ContentTracker");
                    break;
                case "Lists":
                    _eventAggregator.GetEvent<NavigateToEvent>().Publish("ListScreen");
                    break;
                // Добавьте другие экраны по необходимости
                default:
                    _eventAggregator.GetEvent<NavigateToEvent>().Publish(screen);
                    break;
            }
        }

        private void OnCategoryToggle(SidebarCategory category)
        {
            if (category != null)
                category.IsExpanded = !category.IsExpanded;
        }

        private void OnItemClick(MediaEditModel item)
        {
            if (item == null) return;

            // Публикуем событие открытия деталей для выбранного элемента
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(item);
        }
    }
}
