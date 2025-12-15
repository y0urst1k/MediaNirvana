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
            set { SetProperty(ref _items, value); InitializeFilteredView(); UpdateTagsList(); }
        }

        // === 2. Свойства фильтров ===
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); RefreshFilter(); }
        }

        private string _selectedType = "All";
        public string SelectedType
        {
            get => _selectedType;
            set { SetProperty(ref _selectedType, value); RefreshFilter(); }
        }

        private string _selectedYear = "All";
        public string SelectedYear
        {
            get => _selectedYear;
            set { SetProperty(ref _selectedYear, value); RefreshFilter(); }
        }

        private string _selectedTag = "All Tags";
        public string SelectedTag
        {
            get => _selectedTag;
            set { SetProperty(ref _selectedTag, value); RefreshFilter(); }
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

        // Команда для радио-кнопок
        public DelegateCommand<string> SetStatusFilterCommand { get; }
        // Команда для кнопки Clear
        public DelegateCommand ClearFiltersCommand { get; }

        public ContentTrackerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            TagList = new ObservableCollection<string>();

            AddCommand = new DelegateCommand(() => _eventAggregator.GetEvent<AddMediaRequestedEvent>().Publish());
            DeleteCommand = new DelegateCommand<MediaEditModel>(item => _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(item.Id));
            DetailCommand = new DelegateCommand<MediaEditModel>(item => _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(item));

            SetStatusFilterCommand = new DelegateCommand<string>(status =>
            {
                _statusFilter = status;
                RefreshFilter();
            });

            ClearFiltersCommand = new DelegateCommand(() =>
            {
                SearchText = "";
                SelectedType = "All";
                SelectedYear = "All";
                SelectedTag = "All Tags";
                _statusFilter = "All";
                // RefreshFilter вызовется в сеттерах
            });

            // Подписки на внешние изменения данных
            _eventAggregator.GetEvent<MediaAddedEvent>().Subscribe(_ => { UpdateTagsList(); RefreshFilter(); });
            _eventAggregator.GetEvent<ItemUpdatedEvent>().Subscribe(_ => { UpdateTagsList(); RefreshFilter(); });
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Subscribe(_ => RefreshFilter());
        }

        // === ИНИЦИАЛИЗАЦИЯ ===
        // Привязка к внешней коллекции
        public void SetSourceItems(ObservableCollection<MediaEditModel> items)
        {
            _items = items;
            InitializeFilteredView();
            UpdateTagsList();
        }

        private void InitializeFilteredView()
        {
            if (_items == null) return;
            FilteredView = CollectionViewSource.GetDefaultView(_items);
            FilteredView.Filter = FilterItem;
        }

        private void RefreshFilter() => FilteredView?.Refresh();

        // === ЛОГИКА ФИЛЬТРАЦИИ ===
        private bool FilterItem(object obj)
        {
            if (obj is not MediaEditModel item) return false;

            // 1. Status
            if (!string.Equals(_statusFilter, "All", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(item.Status.ToString(), _statusFilter, StringComparison.OrdinalIgnoreCase))
                return false;

            // 2. Search Query
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string query = SearchText.ToLower();
                if ((!item.Title?.ToLower().Contains(query) ?? true) &&
                    (!item.OriginalTitle?.ToLower().Contains(query) ?? true) &&
                    (!item.Country?.ToLower().Contains(query) ?? true))
                    return false;
            }

            // 3. Type
            if (!string.IsNullOrEmpty(SelectedType) &&
                !string.Equals(SelectedType, "All", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(item.Type.ToString(), SelectedType, StringComparison.OrdinalIgnoreCase))
                return false;

            // 4. Year
            if (!string.IsNullOrEmpty(SelectedYear) &&
                !string.Equals(SelectedYear, "All", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(SelectedYear, "Older", StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Year >= 2020) return false;
                }
                else
                {
                    if (!string.Equals(item.Year.ToString(), SelectedYear, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            // 5. Tag
            if (!string.IsNullOrEmpty(SelectedTag) &&
                !string.Equals(SelectedTag, "All Tags", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(item.TagsInput) ||
                    !item.TagsInput.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .Contains(SelectedTag, StringComparer.OrdinalIgnoreCase))
                    return false;
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
            UpdateTagsList(); // Обновляем теги при добавлении
            OnFilterChanged(); // Перефильтровываем
        }

        private void OnItemUpdated(MediaEditModel updatedItem)
        {
            UpdateTagsList(); // Возможно, изменились теги
            OnFilterChanged(); // Перефильтровываем
        }

        private void OnDeleteRequested(Guid mediaId)
        {
            OnFilterChanged(); // Обновляем отображение
        }
    }
}