using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MediaTracker.Views
{
    public partial class SearchFilters : UserControl
    {
        // === PROPS (Dependency Properties) ===

        // 1. SearchQuery
        public static readonly DependencyProperty SearchQueryProperty =
            DependencyProperty.Register("SearchQuery", typeof(string), typeof(SearchFilters),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFilterChanged));

        public string SearchQuery
        {
            get => (string)GetValue(SearchQueryProperty);
            set => SetValue(SearchQueryProperty, value);
        }

        // 2. SelectedType
        public static readonly DependencyProperty SelectedTypeProperty =
            DependencyProperty.Register("SelectedType", typeof(string), typeof(SearchFilters),
                new FrameworkPropertyMetadata("All", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFilterChanged));

        public string SelectedType
        {
            get => (string)GetValue(SelectedTypeProperty);
            set => SetValue(SelectedTypeProperty, value);
        }

        // 3. SelectedYear
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof(string), typeof(SearchFilters),
                new FrameworkPropertyMetadata("All", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFilterChanged));

        public string SelectedYear
        {
            get => (string)GetValue(SelectedYearProperty);
            set => SetValue(SelectedYearProperty, value);
        }

        // 4. SelectedTag
        public static readonly DependencyProperty SelectedTagProperty =
            DependencyProperty.Register("SelectedTag", typeof(string), typeof(SearchFilters),
                new FrameworkPropertyMetadata("All Tags", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFilterChanged));

        public string SelectedTag
        {
            get => (string)GetValue(SelectedTagProperty);
            set => SetValue(SelectedTagProperty, value);
        }

        // 5. AvailableTags (OneWay, только на вход)
        public static readonly DependencyProperty AvailableTagsProperty =
            DependencyProperty.Register("AvailableTags", typeof(IEnumerable<string>), typeof(SearchFilters));

        public IEnumerable<string> AvailableTags
        {
            get => (IEnumerable<string>)GetValue(AvailableTagsProperty);
            set => SetValue(AvailableTagsProperty, value);
        }

        // 6. HasActiveFilters (ReadOnly для UI)
        public static readonly DependencyProperty HasActiveFiltersProperty =
            DependencyProperty.Register("HasActiveFilters", typeof(bool), typeof(SearchFilters), new PropertyMetadata(false));

        public bool HasActiveFilters
        {
            get => (bool)GetValue(HasActiveFiltersProperty);
            private set => SetValue(HasActiveFiltersProperty, value);
        }

        // Событие очистки
        public event RoutedEventHandler ClearRequested;

        public SearchFilters()
        {
            InitializeComponent();
        }

        // Вызывается при изменении любого фильтра
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SearchFilters;
            control?.CheckFilters();
        }

        private void CheckFilters()
        {
            // Логика "hasActiveFilters" из React
            bool hasSearch = !string.IsNullOrEmpty(SearchQuery);
            bool hasType = SelectedType != "All" && SelectedType != null;
            bool hasYear = SelectedYear != "All" && SelectedYear != null;
            bool hasTag = SelectedTag != "All Tags" && SelectedTag != null; // Зависит от дефолтного значения тега

            HasActiveFilters = hasSearch || hasType || hasYear || hasTag;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем значения (TwoWay Binding обновит их в родителе)
            SearchQuery = "";
            SelectedType = "All";
            SelectedYear = "All";
            SelectedTag = "All Tags"; // Или ваше дефолтное значение тега

            ClearRequested?.Invoke(this, e);
        }
    }
}