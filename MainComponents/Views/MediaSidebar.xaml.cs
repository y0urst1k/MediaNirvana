using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MediaTracker.Views
{
    // Класс-обертка для категории (ViewModel)
    public class SidebarCategory : INotifyPropertyChanged
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public List<ContentItem> AllItems { get; set; } = new List<ContentItem>();

        // Показываем только топ-5
        public List<ContentItem> TopItems => AllItems.Take(5).ToList();

        public int Count => AllItems.Count;
        public int RemainingCount => Math.Max(0, Count - 5);
        public bool HasMore => RemainingCount > 0;

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class MediaSidebar : UserControl, INotifyPropertyChanged
    {
        // === Dependency Properties ===

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<ContentItem>), typeof(MediaSidebar),
                new PropertyMetadata(null, OnItemsChanged));

        public ObservableCollection<ContentItem> Items
        {
            get => (ObservableCollection<ContentItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(MediaSidebar));
        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        // === Local State ===
        public ObservableCollection<SidebarCategory> Categories { get; set; } = new ObservableCollection<SidebarCategory>();

        // === Events ===
        public event EventHandler<string> Navigate; // "library" or "lists"
        public event EventHandler<ContentItem> ItemClick;

        public MediaSidebar()
        {
            InitializeComponent();
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sidebar = d as MediaSidebar;
            if (sidebar != null)
            {
                sidebar.RebuildCategories();
                // Подписываемся на изменения коллекции, чтобы перестраивать меню
                if (e.NewValue is ObservableCollection<ContentItem> newCol)
                {
                    newCol.CollectionChanged += (s, ev) => sidebar.RebuildCategories();
                }
            }
        }

        private void RebuildCategories()
        {
            if (Items == null) return;

            // Определяем категории как в React
            var defs = new[]
            {
                new { Type="Book", Label="Books" },
                new { Type="Movie", Label="Movies" },
                new { Type="Show", Label="TV Shows" },
                new { Type="Music", Label="Music" },
                new { Type="Game", Label="Games" },
                new { Type="Other", Label="Other" }
            };

            // Сохраняем состояние раскрытия
            var expandedTypes = Categories.Where(c => c.IsExpanded).Select(c => c.Type).ToList();
            // По умолчанию раскрыты (как в React: Movie, Show, Book)
            if (Categories.Count == 0) expandedTypes.AddRange(new[] { "Movie", "Show", "Book" });

            Categories.Clear();

            foreach (var def in defs)
            {
                var itemsInCat = Items.Where(i => i.Type == def.Type).ToList();

                var cat = new SidebarCategory
                {
                    Type = def.Type,
                    Label = def.Label,
                    AllItems = itemsInCat,
                    IsExpanded = expandedTypes.Contains(def.Type)
                };
                Categories.Add(cat);
            }
        }

        // === Handlers ===

        private void Nav_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                string screen = rb.Tag.ToString(); // "library" or "lists"
                Navigate?.Invoke(this, screen);
            }
        }

        private void CategoryHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is SidebarCategory cat)
            {
                cat.IsExpanded = !cat.IsExpanded;
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ContentItem item)
            {
                ItemClick?.Invoke(this, item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}