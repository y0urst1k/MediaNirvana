using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MediaTracker.Models; // <--- ВАЖНО: Подключаем модели

namespace MediaTracker.Views
{
    // Вспомогательный класс для отображения (ViewModel одной карточки)
    public class StatCardData
    {
        public string Label { get; set; }
        public int Count { get; set; }
        public Geometry IconGeometry { get; set; }
        public Brush ForegroundColor { get; set; }
        public Brush BackgroundColor { get; set; }
        public Color HoverColor { get; set; } // Для анимации рамки
    }

    public partial class StatsOverview : UserControl
    {
        // Входные данные: Список всех элементов
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<ContentItem>), typeof(StatsOverview),
                new PropertyMetadata(null, OnItemsChanged));

        public ObservableCollection<ContentItem> Items
        {
            get => (ObservableCollection<ContentItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        // Коллекция для биндинга в XAML
        public ObservableCollection<StatCardData> StatCards { get; set; } = new ObservableCollection<StatCardData>();

        public StatsOverview()
        {
            InitializeComponent();
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StatsOverview;
            if (control != null)
            {
                control.RecalculateStats();

                // Если коллекция изменится (добавили элемент), нужно пересчитать
                if (e.NewValue is ObservableCollection<ContentItem> newCol)
                {
                    newCol.CollectionChanged += (s, ev) => control.RecalculateStats();
                }
            }
        }

        private void RecalculateStats()
        {
            if (Items == null) return;

            // 1. Считаем количество (Reduce)
            var counts = Items
                .GroupBy(x => x.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            StatCards.Clear();

            // 2. Создаем карточки (Map config)
            // Порядок: Planned, Watching, Reading, Completed, Abandoned, Paused

            AddCard(InteractionStatus.Planned, counts,
                "M19 4h-1V2h-2v2H8V2H6v2H5c-1.11 0-1.99.9-1.99 2L3 20c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 16H5V10h14v10zm0-12H5V6h14v2z", // Calendar
                "#475569", "#F1F5F9"); // Slate

            AddCard(InteractionStatus.Watching, counts,
                "M8 5v14l11-7z", // Play
                "#2563EB", "#DBEAFE"); // Blue

            AddCard(InteractionStatus.Reading, counts,
                "M18 2H6c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2zM6 4h5v8l-2.5-1.5L6 12V4z", // Book (Simplified)
                "#9333EA", "#F3E8FF"); // Purple

            AddCard(InteractionStatus.Completed, counts,
                "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z", // CheckCircle
                "#16A34A", "#DCFCE7"); // Green

            AddCard(InteractionStatus.Abandoned, counts,
                "M12 2C6.47 2 2 6.47 2 12s4.47 10 10 10 10-4.47 10-10S17.53 2 12 2zm5 13.59L15.59 17 12 13.41 8.41 17 7 15.59 10.59 12 7 8.41 8.41 7 12 10.59 15.59 7 17 8.41 13.41 12 17 15.59z", // X Circle
                "#DC2626", "#FEE2E2"); // Red

            AddCard(InteractionStatus.Paused, counts,
                "M6 19h4V5H6v14zm8-14v14h4V5h-4z", // Pause
                "#D97706", "#FEF3C7"); // Amber
        }

        private void AddCard(InteractionStatus status, Dictionary<InteractionStatus, int> counts, string pathData, string fgHex, string bgHex)
        {
            int count = counts.ContainsKey(status) ? counts[status] : 0;

            // Парсим цвета
            var fgColor = (Color)ColorConverter.ConvertFromString(fgHex);
            var bgColor = (Color)ColorConverter.ConvertFromString(bgHex);

            StatCards.Add(new StatCardData
            {
                Label = status.ToString(),
                Count = count,
                IconGeometry = Geometry.Parse(pathData),
                ForegroundColor = new SolidColorBrush(fgColor),
                BackgroundColor = new SolidColorBrush(bgColor),
                HoverColor = fgColor // При наведении рамка станет цвета текста
            });
        }
    }
}