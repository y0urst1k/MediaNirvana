namespace MainComponents.ViewModels
{
    public class SearchFiltersViewModel : BindableBase
    {
        // === Свойства фильтров ===
        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
                CheckFilters();
            }
        }

        private string _selectedType = "All";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);
                CheckFilters();
            }
        }

        private string _selectedYear = "All";
        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                SetProperty(ref _selectedYear, value);
                CheckFilters();
            }
        }

        private string _selectedTag = "All Tags";
        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                SetProperty(ref _selectedTag, value);
                CheckFilters();
            }
        }

        private IEnumerable<string> _availableTags;
        public IEnumerable<string> AvailableTags
        {
            get => _availableTags;
            set => SetProperty(ref _availableTags, value);
        }

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
        private void CheckFilters()
        {
            bool hasSearch = !string.IsNullOrEmpty(SearchQuery);
            bool hasType = SelectedType != "All" && SelectedType != null;
            bool hasYear = SelectedYear != "All" && SelectedYear != null;
            bool hasTag = SelectedTag != "All Tags" && SelectedTag != null;

            HasActiveFilters = hasSearch || hasType || hasYear || hasTag;
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
