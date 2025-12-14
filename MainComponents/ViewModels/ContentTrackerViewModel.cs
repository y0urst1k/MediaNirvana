using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Infrastructure.DTO;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ContentTrackerViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        // === 1. Входные данные (привязываются из View) ===
        private ObservableCollection<MediaEditModel> _items;
        public ObservableCollection<MediaEditModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        // === 2. Свойства фильтров ===
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value, OnFilterChanged);
        }

        private string _selectedType = "All";
        public string SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value, OnFilterChanged);
        }

        private string _selectedYear = "All";
        public string SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value, OnFilterChanged);
        }

        private string _selectedTag = "All Tags";
        public string SelectedTag
        {
            get => _selectedTag;
            set => SetProperty(ref _selectedTag, value, OnFilterChanged);
        }

        // Список доступных тегов (вычисляется автоматически)
        private ObservableCollection<string> _tagList;
        public ObservableCollection<string> TagList
        {
            get => _tagList;
            set => SetProperty(ref _tagList, value);
        }

        // Текущий статус фильтра (из RadioButtons)
        private string _statusFilter = "All";
        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                SetProperty(ref _statusFilter, value);
                FilteredView?.Refresh();
            }
        }

        // === 3. Фильтрованный список для UI ===
        private ICollectionView _filteredView;
        public ICollectionView FilteredView
        {
            get => _filteredView;
            private set => SetProperty(ref _filteredView, value);
        }

        // === 4. Команды (вместо RoutedEvents) ===
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<MediaEditModel> DeleteCommand { get; }
        public DelegateCommand<MediaEditModel> DetailCommand { get; }

        public ContentTrackerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            // Инициализация команд
            AddCommand = new DelegateCommand(OnAdd);
            DeleteCommand = new DelegateCommand<MediaEditModel>(OnDelete);
            DetailCommand = new DelegateCommand<MediaEditModel>(OnDetail);

            // Инициализация коллекции тегов
            TagList = new ObservableCollection<string>();

            _eventAggregator.GetEvent<MediaAddedEvent>().Subscribe(OnMediaAdded);
        }

        // === ИНИЦИАЛИЗАЦИЯ ===
        public void Initialize()
        {
            if (Items == null) return;

            // Создаём фильтрованный View
            FilteredView = CollectionViewSource.GetDefaultView(Items);
            FilteredView.Filter = FilterItem;

            // Собираем теги
            UpdateTagsList();

            // Следим за изменениями в Items
            Items.CollectionChanged += (s, e) => UpdateTagsList();
        }

        // === ЛОГИКА ФИЛЬТРАЦИИ ===
        private bool FilterItem(object obj)
        {
            if (obj is not MediaEditModel item) return false;

            // 1. Status
            if (_statusFilter != "All" && item.Status.ToString() != _statusFilter) return false;

            // 2. Search Query
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string query = SearchText.ToLower();
                bool matchesTitle = item.Title?.ToLower().Contains(query) ?? false;
                bool matchesOrig = item.OriginalTitle?.ToLower().Contains(query) ?? false;
                bool matchesCountry = item.Country?.ToLower().Contains(query) ?? false;


                if (!matchesTitle && !matchesOrig && !matchesCountry) return false;
            }

            // 3. Type
            if (!string.IsNullOrEmpty(SelectedType) && SelectedType != "All" && item.Type.ToString() != SelectedType) return false;

            // 4. Year
            if (!string.IsNullOrEmpty(SelectedYear) && SelectedYear != "All")
            {
                if (SelectedYear == "Older")
                {
                    if (item.Year >= 2020) return false;
                }
                else
                {
                    if (item.Year.ToString() != SelectedYear) return false;
                }
            }

            // 5. Tag
            if (!string.IsNullOrEmpty(SelectedTag) && SelectedTag != "All Tags")
            {
                if (item.TagsInput == null || !item.TagsInput.Contains(SelectedTag)) return false;
            }

            return true;
        }

        // Вызывается при изменении любого фильтра
        private void OnFilterChanged()
        {
            FilteredView?.Refresh();
        }

        // === ОБРАБОТЧИКИ КОМАНД ===
        private void OnAdd()
        {
            // Можно опубликовать событие через EventAggregator
            _eventAggregator.GetEvent<AddMediaRequestedEvent>().Publish();
        }

        private void OnDelete(MediaEditModel item)
        {
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(item.Id);
        }

        private void OnDetail(MediaEditModel item)
        {
            if (item == null) return;
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(item);
        }

        // Смена статуса (из RadioButton)
        public void SetStatusFilter(string status)
        {
            _statusFilter = status;
            FilteredView?.Refresh();
        }

        // Сброс фильтров
        public void ClearFilters()
        {
            SearchText = "";
            SelectedType = "All";
            SelectedYear = "All";
            SelectedTag = "All Tags";
            _statusFilter = "All";
            FilteredView?.Refresh();
        }

        // Обновление списка тегов
        private void UpdateTagsList()
        {
            if (Items == null) return;

            var tags = Items
                .Where(item => !string.IsNullOrWhiteSpace(item.TagsInput))
                .SelectMany(item => item.TagsInput
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase) // регистронезависимая сортировка
                .ToList();

            TagList.Clear();
            foreach (var tag in tags)
            {
                TagList.Add(tag);
            }
        }

        private void OnMediaAdded(MediaEditModel model)
        {
            // Добавляем в коллекцию
            Items.Add(model);

            // Можно обновить фильтры/поиск
            UpdateFilteredView();
        }

        private void UpdateFilteredView()
        {
            // Проверяем, инициализирован ли FilteredView
            if (FilteredView == null)
            {
                // Если нет — создаём новый ICollectionView
                FilteredView = CollectionViewSource.GetDefaultView(Items);
                FilteredView.Filter = FilterItem;
            }
            else
            {
                // Если уже есть — просто обновляем данные
                FilteredView.Refresh();
            }

            // Обновляем список тегов (так как могли добавиться новые теги в новом элементе)
            UpdateTagsList();

            // Дополнительно можно обновить состояние фильтров, если нужно
            OnFilterChanged();
        }
    }
}