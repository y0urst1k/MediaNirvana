using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; 

namespace MainComponents.Views
{
    public partial class ContentTrackerView : UserControl, INotifyPropertyChanged
    {
        // === 1. Входные данные (Items) ===
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<ContentItem>), typeof(ContentTrackerView),
                new PropertyMetadata(null, OnItemsChanged));
        public ObservableCollection<ContentItem> Items
        {
            get { return (ObservableCollection<ContentItem>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
        }
        // === 2. Свойства фильтров (связаны с SearchFilters) ===
        // SearchText
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(ContentTrackerView),
                new PropertyMetadata("", OnFilterPropChanged));
        public string SearchText { get => (string)GetValue(SearchTextProperty); set => SetValue(SearchTextProperty, value); }
        
        // SelectedType
        public static readonly DependencyProperty SelectedTypeProperty =
            DependencyProperty.Register("SelectedType", typeof(string), typeof(ContentTrackerView),
                new PropertyMetadata("All", OnFilterPropChanged));
        public string SelectedType { get => (string)GetValue(SelectedTypeProperty); set => SetValue(SelectedTypeProperty, value); }
        
        // SelectedYear
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof(string), typeof(ContentTrackerView),
                new PropertyMetadata("All", OnFilterPropChanged));
        public string SelectedYear { get => (string)GetValue(SelectedYearProperty); set => SetValue(SelectedYearProperty, value); }
        
        // SelectedTag
        public static readonly DependencyProperty SelectedTagProperty =
            DependencyProperty.Register("SelectedTag", typeof(string), typeof(ContentTrackerView),
                new PropertyMetadata("All Tags", OnFilterPropChanged)); 
        
        // Дефолт должен совпадать с SearchFilters
        public string SelectedTag { get => (string)GetValue(SelectedTagProperty); set => SetValue(SelectedTagProperty, value); }
        
        // Список доступных тегов (вычисляется автоматически)
        public ObservableCollection<string> TagList { get; set; } = new ObservableCollection<string>();


        // === 3. Внутреннее состояние ===
        
        // Представление для фильтрации
        private ICollectionView _filteredView;
        public ICollectionView FilteredView
        {
            get => _filteredView;
            set { _filteredView = value; OnPropertyChanged("FilteredView"); }
        }

        // Текущий статус фильтра (из RadioButtons)
        private string _statusFilter = "All";

        // === 4. События ===
        public event RoutedEventHandler AddRequested;
        public event ContentCard.CardActionHandler ItemDeleted;
        public event ContentCard.CardActionHandler ItemViewDetail;

        public ContentTrackerView()
        {
            InitializeComponent();
        }

        // === ИНИЦИАЛИЗАЦИЯ ===
        
        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ContentTrackerView;
            if (control != null && e.NewValue != null)
            {
                control.SetupView();
            }
        }

        private void SetupView()
        {
            if (Items == null) return;

            // 1. Создаем View
            FilteredView = CollectionViewSource.GetDefaultView(Items);
            
            // 2. Устанавливаем логику фильтрации
            FilteredView.Filter = FilterItem;

            // 3. Первичный сбор тегов
            UpdateTagsList();

            // 4. Следим за изменениями в коллекции (добавление/удаление элементов)
            Items.CollectionChanged += (s, e) => {
                UpdateTagsList();
                // При добавлении элемента фильтр может его скрыть, если он не подходит, это нормально.
            };
        }

        // === ЛОГИКА ФИЛЬТРАЦИИ ===
        
        // Этот метод вызывается для КАЖДОГО элемента. Если вернет false - элемент скрыт.
        private bool FilterItem(object obj)
        {
            if (obj is not ContentItem item) return false;

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
            // Убеждаемся, что SelectedType не null и не "All"
            if (!string.IsNullOrEmpty(SelectedType) && SelectedType != "All" && item.Type != SelectedType) return false;

            // 4. Year
            if (!string.IsNullOrEmpty(SelectedYear) && SelectedYear != "All")
            {
                if (SelectedYear == "Older")
                {
                    if (item.Year >= 2020) return false; // Логика "Старше" как в React примере
                }
                else
                {
                    if (item.Year.ToString() != SelectedYear) return false;
                }
            }

            // 5. Tag
            // "All Tags" - это значение по умолчанию в SearchFilters
            if (!string.IsNullOrEmpty(SelectedTag) && SelectedTag != "All Tags")
            {
                if (item.Tags == null || !item.Tags.Contains(SelectedTag)) return false;
            }

            return true;
        }

        // === ОБРАБОТЧИКИ ИЗМЕНЕНИЙ ===

        // Вызывается при изменении любого DependencyProperty фильтров (SearchText, Type, etc.)
        private static void OnFilterPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Заставляем View пересчитать фильтры
            (d as ContentTrackerView)?.FilteredView?.Refresh();
        }

        // Вызывается при клике на RadioButton статуса
        private void OnStatusChanged(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                _statusFilter = rb.Tag.ToString();
                FilteredView?.Refresh();
            }
        }

        // Вызывается кнопкой Clear в SearchFilters
        private void OnClearFilters(object sender, RoutedEventArgs e)
        {
            // Мы просто сбрасываем статус, остальные поля сбросятся через TwoWay binding в SearchFilters
            // Но RadioButton нужно сбросить вручную
            
            // Находим RadioButton "All" и нажимаем его
            // (Простой способ: просто сбрасываем переменную и рефрешим, но UI радиокнопок не обновится)
            // Поэтому переберем UI (это немного грязно, но работает для UserControl)
            
            _statusFilter = "All";
            
            // Сброс визуального состояния RadioButton (найти кнопку All)
            foreach(var child in ((WrapPanel)((Grid)Content).Children[2]).Children)
            {
                if(child is RadioButton rb && rb.Tag.ToString() == "All")
                {
                    rb.IsChecked = true;
                    break;
                }
            }

            FilteredView?.Refresh();
        }

        private void UpdateTagsList()
        {
            if (Items == null) return;

            var tags = Items
                .Where(x => x.Tags != null)
                .SelectMany(x => x.Tags)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            TagList.Clear();
            // В SearchFilters первый элемент комбобокса обычно захардкожен как "All Tags", 
            // но мы можем передавать только список доступных.
            foreach (var t in tags) TagList.Add(t);
        }

        // === PROXY EVENTS (Проброс событий наверх) ===
        private void OnAdd_Click(object sender, RoutedEventArgs e) => AddRequested?.Invoke(this, e);
        private void OnItemDeleted(object sender, ContentItem item) => ItemDeleted?.Invoke(this, item);
        private void OnItemDetail(object sender, ContentItem item) => ItemViewDetail?.Invoke(this, item);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}