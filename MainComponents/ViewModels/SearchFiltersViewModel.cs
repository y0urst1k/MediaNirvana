using Infrastructure.DTO;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class SearchFiltersViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        // === Свойства фильтров ===
        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
                UpdateFilters();
            }
        }

        private string _selectedType = "All";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);
                UpdateFilters();
            }
        }

        private string _selectedYear = "All";
        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                SetProperty(ref _selectedYear, value);
                UpdateFilters();
            }
        }

        private string _selectedTag = "All Tags";
        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                SetProperty(ref _selectedTag, value);
                UpdateFilters();
            }
        }
        public IEnumerable<string> Types { get; } = new[] { "All", "Book", "Movie", "Series", "Game" };
        public IEnumerable<string> Years { get; } = new[] { "All", "2024", "2023", "2022", "Old" };
        public IEnumerable<string> AvailableTags { get; set; }

        // === Флаг активных фильтров ===
        private bool _hasActiveFilters;
        public bool HasActiveFilters
        {
            get => _hasActiveFilters;
            private set => SetProperty(ref _hasActiveFilters, value);
        }

        // === Команда очистки ===
        public DelegateCommand ClearCommand { get; }

        public SearchFiltersViewModel(IEventAggregator eventAggregator)
        {
            ClearCommand = new DelegateCommand(ExecuteClear);
        }

        // === Логика проверки фильтров ===
        private void UpdateFilters()
        {
            // 1. Обновляем флаг UI
            bool hasSearch = !string.IsNullOrEmpty(SearchQuery);
            bool hasType = SelectedType != "All";
            bool hasYear = SelectedYear != "All";
            bool hasTag = SelectedTag != "All Tags";

            HasActiveFilters = hasSearch || hasType || hasYear || hasTag;

            // 2. Публикуем событие для списка медиа
            var filterState = new FilterState
            {
                Query = SearchQuery,
                Type = SelectedType,
                Year = SelectedYear,
                Tag = SelectedTag
            };

            _eventAggregator.GetEvent<FilterChangedEvent>().Publish(filterState);
        }

        // === Обработка очистки ===
        private void ExecuteClear()
        {
            SearchQuery = "";
            SelectedType = "All";
            SelectedYear = "All";
            SelectedTag = "All Tags";
        }
    }
}
