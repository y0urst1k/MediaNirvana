using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaTracker.Views
{
    public partial class ListsScreen : UserControl
    {
        // Входные данные: Списки
        public static readonly DependencyProperty ListsProperty =
            DependencyProperty.Register("Lists", typeof(ObservableCollection<PersonalList>), typeof(ListsScreen));
        public ObservableCollection<PersonalList> Lists
        {
            get => (ObservableCollection<PersonalList>)GetValue(ListsProperty);
            set => SetValue(ListsProperty, value);
        }

        // Входные данные: Все элементы (нужны для отображения деталей)
        public static readonly DependencyProperty AllItemsProperty =
            DependencyProperty.Register("AllItems", typeof(ObservableCollection<ContentItem>), typeof(ListsScreen));
        public ObservableCollection<ContentItem> AllItems
        {
            get => (ObservableCollection<ContentItem>)GetValue(AllItemsProperty);
            set => SetValue(AllItemsProperty, value);
        }

        private PersonalList _selectedList;

        public ListsScreen()
        {
            InitializeComponent();
        }

        // === Navigation ===
        private void List_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is PersonalList list)
            {
                OpenListDetail(list);
            }
        }

        private void CloseDetail_Click(object sender, RoutedEventArgs e)
        {
            _selectedList = null;
            ListsGrid.Visibility = Visibility.Visible;
            ListDetail.Visibility = Visibility.Collapsed;
        }

        private void OpenListDetail(PersonalList list)
        {
            _selectedList = list;
            DetailName.Text = list.Name;
            DetailDesc.Text = list.Description;
            RefreshDetailItems();

            ListsGrid.Visibility = Visibility.Collapsed;
            ListDetail.Visibility = Visibility.Visible;
        }

        private void RefreshDetailItems()
        {
            if (_selectedList == null || AllItems == null) return;

            // Находим реальные объекты ContentItem по их ID
            var itemsInList = AllItems
                .Where(item => _selectedList.ItemIds.Contains(item.Id))
                .ToList();

            DetailItemsControl.ItemsSource = itemsInList;
        }

        // === CRUD Logic ===

        private void CreateList_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ListEditorDialog();
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                Lists.Add(dialog.ResultList);
            }
        }

        private void EditList_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // Чтобы не сработал клик по карточке
            if (sender is FrameworkElement fe && fe.DataContext is PersonalList list)
            {
                var dialog = new ListEditorDialog(list);
                dialog.Owner = Window.GetWindow(this);
                if (dialog.ShowDialog() == true)
                {
                    // Обновляем поля
                    list.Name = dialog.ResultList.Name;
                    list.Description = dialog.ResultList.Description;
                    list.Type = dialog.ResultList.Type;
                    // В реальном MVVM нужен метод Update
                }
            }
        }

        private void DeleteList_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sender is FrameworkElement fe && fe.DataContext is PersonalList list)
            {
                if (MessageBox.Show($"Delete list '{list.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Lists.Remove(list);
                }
            }
        }

        // === Managing Items in List ===

        private void ManageItems_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedList == null) return;

            // Открываем диалог выбора
            var dialog = new ListItemsDialog(AllItems, _selectedList.ItemIds);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                // Обновляем список ID
                _selectedList.ItemIds = dialog.SelectedIds;
                RefreshDetailItems();
            }
        }

        private void RemoveItemFromList_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ContentItem item)
            {
                _selectedList.ItemIds.Remove(item.Id);
                RefreshDetailItems();
            }
        }
    }
}