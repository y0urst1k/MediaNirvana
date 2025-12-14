using System.Collections.ObjectModel;
using System.Windows.Input;
using Infrastructure.DTO;

namespace Dialogs.ViewModels
{
    public class ListItemDialogViewModel : BindableBase, IDialogAware
    {
        private IEnumerable<MediaEditModel> _allItems;
        private List<Guid> _currentIds;

        public ObservableCollection<SelectableItem> Items { get; private set; } = new();
        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value, ApplyFilter);
        }
        private string _searchQuery;

        public ICommand DoneCommand { get; }
        public ICommand CancelCommand { get; }

        public string Title => "Select Items";
        public DialogCloseListener RequestClose { get; set; }


        // Пустой конструктор — Prism создаст VM через контейнер
        public ListItemDialogViewModel()
        {
            DoneCommand = new DelegateCommand(OnDone);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        // Получаем параметры при открытии диалога
        public void OnDialogOpened(IDialogParameters parameters)
        {
            _allItems = parameters.GetValue<IEnumerable<MediaEditModel>>("allItems");
            _currentIds = parameters.GetValue<List<Guid>>("currentIds");

            // Инициализация Items
            Items = new ObservableCollection<SelectableItem>(
                _allItems.Select(item => new SelectableItem
                {
                    Item = item,
                    IsSelected = _currentIds.Contains(item.Id)
                })
            );
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                Items = new ObservableCollection<SelectableItem>(
                    _allItems.Select(item => new SelectableItem
                    {
                        Item = item,
                        IsSelected = _currentIds.Contains(item.Id)
                    })
                );
                return;
            }

            var filtered = _allItems
                .Where(item => item.Title.ToLower().Contains(_searchQuery.ToLower()))
                .Select(item => new SelectableItem
                {
                    Item = item,
                    IsSelected = Items.FirstOrDefault(s => s.Item.Id == item.Id)?.IsSelected ?? false
                });

            Items = new ObservableCollection<SelectableItem>(filtered);
        }

        private void OnDone()
        {
            var selectedIds = Items
                .Where(x => x.IsSelected)
                .Select(x => x.Item.Id)
                .ToList();

            var parameters = new DialogParameters { { "selectedIds", selectedIds } };

            var result = new DialogResult(ButtonResult.OK) { Parameters = parameters };

            RequestClose.Invoke(result);
        }

        private void OnCancel() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));


        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
    }
}
