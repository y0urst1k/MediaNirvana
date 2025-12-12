using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MediaTracker.Views
{
    // Вспомогательный класс для CheckBox
    public class SelectableItem
    {
        public ContentItem Item { get; set; }
        public bool IsSelected { get; set; }
    }

    public partial class ListItemsDialog : Window
    {
        private List<SelectableItem> _allSelectables;

        public List<string> SelectedIds => _allSelectables
            .Where(x => x.IsSelected)
            .Select(x => x.Item.Id)
            .ToList();

        public ListItemsDialog(IEnumerable<ContentItem> allItems, List<string> currentIds)
        {
            InitializeComponent();

            // Создаем обертки для каждого элемента
            _allSelectables = allItems.Select(item => new SelectableItem
            {
                Item = item,
                IsSelected = currentIds.Contains(item.Id)
            }).ToList();

            ItemsList.ItemsSource = _allSelectables;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            ItemsList.ItemsSource = _allSelectables.Where(x =>
                x.Item.Title.ToLower().Contains(query)).ToList();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}