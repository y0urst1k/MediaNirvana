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
            set
            {
                SetProperty(ref _items, value);
                RebuildCategories();
            } 
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
            Items = new ObservableCollection<MediaEditModel>();

            NavigateCommand = new DelegateCommand<string>(OnNavigate);
            CategoryToggleCommand = new DelegateCommand<SidebarCategory>(OnCategoryToggle);
            ItemClickCommand = new DelegateCommand<MediaEditModel>(OnItemClick);

            // 2. Подписка на события (для будущих изменений)
            _eventAggregator.GetEvent<MediaAddedEvent>().Subscribe(OnMediaAdded);

            // 2. Подписка на обновление
            _eventAggregator.GetEvent<ItemUpdatedEvent>().Subscribe(OnMediaUpdated);

            // 3. Подписка на удаление
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Subscribe(OnMediaDeleted);

            _eventAggregator.GetEvent<MediaLibraryLoadedEvent>().Subscribe(OnLibraryLoaded);
        }

        private void OnLibraryLoaded(ObservableCollection<MediaEditModel> items)
        {
            // Важно не заменять коллекцию Items полностью, если она привязана, 
            // но лучше просто скопировать или присвоить ссылку, если View умеет обновляться.
            Items = items;
            // RebuildCategories вызовется автоматически в сеттере Items
        }

        private void OnMediaAdded(MediaEditModel newItem)
        {
            if (newItem == null) return;

            // 1. Добавляем в общий список
            Items.Add(newItem);

            // 2. Точечно добавляем в нужную категорию, не перестраивая всё дерево
            AddToCategory(newItem);
        }

        // === ЛОГИКА ОБНОВЛЕНИЯ (UPDATE) ===
        private void OnMediaUpdated(MediaEditModel updatedModel)
        {
            // 1. Находим элемент в общем списке
            var existingItem = Items.FirstOrDefault(i => i.Id == updatedModel.Id);
            if (existingItem == null) return;

            // 2. Проверяем, изменился ли тип (например, Book -> Movie). 
            // Если да, нужно перенести в другую категорию.
            bool typeChanged = existingItem.Type != updatedModel.Type;

            // 3. Обновляем свойства существующего объекта в коллекции
            // Не заменяем сам объект, чтобы не ломать привязки, а копируем свойства
            UpdateExistingItem(existingItem, updatedModel);

            if (typeChanged)
            {
                // Если тип сменился, проще удалить из старой категории и добавить в новую
                RemoveFromCategory(existingItem.Id); // Удалит из старой категории по ID
                AddToCategory(existingItem);         // Добавит в новую на основе нового Type
            }
            else
            {
                // Если тип тот же, просто обновляем визуальное представление внутри категории
                // (Так как мы обновили existingItem по ссылке, а он лежит внутри категории, 
                // UI обновится сам, если реализован INotifyPropertyChanged)
            }
        }

        // === ЛОГИКА УДАЛЕНИЯ (DELETE) ===
        private void OnMediaDeleted(Guid id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                Items.Remove(item);
                RemoveFromCategory(id);
            }
        }

        // === ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ===

        private void AddToCategory(MediaEditModel item)
        {
            var category = Categories.FirstOrDefault(c => c.Type == item.Type);
            if (category == null)
            {
                category = new SidebarCategory { Type = item.Type, Label = GetLabelForType(item.Type), IsExpanded = true };
                Categories.Add(category);
            }
            category.AddItem(item);
        }

        private void RemoveFromCategory(Guid id)
        {
            // Ищем категорию, в которой лежит этот элемент
            foreach (var cat in Categories)
            {
                var itemInCat = cat.AllItems.FirstOrDefault(i => i.Id == id);
                if (itemInCat != null)
                {
                    cat.RemoveItem(id);
                     if (cat.Count == 0) Categories.Remove(cat);
                    break;
                }
            }
        }

        private void UpdateExistingItem(MediaEditModel target, MediaEditModel source)
        {
            target.Title = source.Title;
            target.Type = source.Type;
            target.Status = source.Status;
            // ... скопируйте остальные поля, влияющие на отображение в сайдбаре ...
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
            if (!expandedTypes.Any() && Categories.Count == 0)
                expandedTypes.AddRange(new[] { MediaType.Film, MediaType.Series, MediaType.Book });

            var newCategories = new ObservableCollection<SidebarCategory>();

            foreach (var type in categoryTypes)
            {
                var itemsInCat = Items.Where(i => i.Type == type).ToList();

                // Добавляем категорию, даже если она пустая (опционально, зависит от дизайна)
                if(itemsInCat.Count == 0) continue; 

                var cat = new SidebarCategory
                {
                    Type = type,
                    Label = GetLabelForType(type),
                    AllItems = new ObservableCollection<MediaEditModel>(itemsInCat),
                    //IsExpanded = expandedTypes.Contains(type)
                    IsExpanded = true
                };
                newCategories.Add(cat);
            }

            Categories = newCategories;
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
        private void OnNavigate(string screen) => _eventAggregator.GetEvent<NavigateToEvent>().Publish(screen);

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
