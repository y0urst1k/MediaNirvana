using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Infrastructure.DTO;
using Infrastructure.Interface;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ContentTrackerViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IEditContentDialogService _editDialogService;

        // === ДОЧЕРНИЕ VIEWMODELS ===
        public StatsOverviewViewModel StatsVM { get; }
        public SearchFiltersViewModel FiltersVM { get; }

        // === ДАННЫЕ ===
        private ObservableCollection<MediaEditModel> _allItemsSource; // Полный список

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

        // Состояние фильтра (получаем от FiltersVM через событие)
        private FilterState _currentFilters = new FilterState();

        // === 4. Команды (вместо RoutedEvents) ===
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<MediaEditModel> DeleteCommand { get; }
        public DelegateCommand<MediaEditModel> DetailCommand { get; }
        public DelegateCommand<MediaEditModel> EditCommand { get; }

        // Команда для радио-кнопок
        public DelegateCommand<string> SetStatusFilterCommand { get; }
        // Команда для кнопки Clear
        public DelegateCommand ClearFiltersCommand { get; }

        public ContentTrackerViewModel(IEventAggregator eventAggregator, IEditContentDialogService editDialogService)
        {
            _eventAggregator = eventAggregator;
            TagList = new ObservableCollection<string>();
            _editDialogService = editDialogService;

            // 1. Инициализация дочерних VM
            StatsVM = new StatsOverviewViewModel();
            FiltersVM = new SearchFiltersViewModel(eventAggregator);

            AddCommand = new DelegateCommand(() => _eventAggregator.GetEvent<AddMediaRequestedEvent>().Publish());
            DeleteCommand = new DelegateCommand<MediaEditModel>(OnDelete);
            DetailCommand = new DelegateCommand<MediaEditModel>(OnDetail);
            EditCommand = new DelegateCommand<MediaEditModel>(OnEdit);

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
            _allItemsSource = items;

            // 1. Настраиваем View для списка
            FilteredView = CollectionViewSource.GetDefaultView(_allItemsSource);
            FilteredView.Filter = FilterItem;

            // 2. Передаем данные в Статистику (она сама посчитает цифры)
            StatsVM.Items = _allItemsSource;

            // 3. Вычисляем теги и отдаем в Фильтр
            UpdateAvailableTags();

            // Подписываемся на изменения в исходной коллекции, чтобы обновлять теги
            _allItemsSource.CollectionChanged += (s, e) => UpdateAvailableTags();
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

            // Проверка табов статуса (верхний уровень)
            if (_statusFilter != "All" && item.Status.ToString() != _statusFilter) return false;

            // Проверка фильтров из дочерней VM
            if (!_currentFilters.IsActive) return true;

            if (!string.IsNullOrEmpty(_currentFilters.Query))
            {
                if (!item.Title.Contains(_currentFilters.Query, StringComparison.OrdinalIgnoreCase)) return false;
            }

            if (_currentFilters.Type != "All" && item.Type.ToString() != _currentFilters.Type) return false;
            if (_currentFilters.Year != "All")
            {
                if (_currentFilters.Year == "Old" && item.Year >= 2020) return false;
                if (_currentFilters.Year != "Old" && item.Year?.ToString() != _currentFilters.Year) return false;
            }
            if (_currentFilters.Tag != "All Tags" && !item.TagsInput.Contains(_currentFilters.Tag)) return false;

            return true;
        }

        // Вызывается при изменении любого фильтра
        private void OnFilterChanged()
        {
            FilteredView?.Refresh();
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

        private void OnDetail(MediaEditModel item) => _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(item);

        private void OnDelete(MediaEditModel item)
        {
            // Можно тут вызвать MessageBox или сервис диалогов перед удалением
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(item.Id);
        }

        private async void OnEdit(MediaEditModel item)
        {
            var updatedItem = await _editDialogService.ShowEditDialogAsync(item);
            if (updatedItem != null)
            {
                // Обновляем поля существующего объекта, чтобы UI обновился
                // (Лучше использовать AutoMapper или метод копирования)
                item.Title = updatedItem.Title;
                item.Status = updatedItem.Status;
                item.UserNotes = updatedItem.UserNotes;
                // ... остальные поля ...

                _eventAggregator.GetEvent<ItemUpdatedEvent>().Publish(item);
            }
        }

        private void UpdateAvailableTags()
        {
            if (_allItemsSource == null) return;

            var tags = _allItemsSource
                .SelectMany(x => x.TagsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            tags.Insert(0, "All Tags");

            // Передаем в дочернюю VM
            FiltersVM.AvailableTags = tags;
        }
    }
}