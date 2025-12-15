using System.Collections.ObjectModel;
using Infrastructure.DTO;

namespace MainComponents.ViewModels
{
    public class ListScreenViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        // Данные
        public ObservableCollection<PersonalListEditModel> Lists { get; } = new();
        public ObservableCollection<MediaEditModel> AllItems { get; } = new();

        // Текущно выбранный список
        private PersonalListEditModel _selectedList;
        public PersonalListEditModel SelectedList
        {
            get => _selectedList;
            set => SetProperty(ref _selectedList, value);
        }

        // Для Detail-панели
        public string DetailName { get; private set; }
        public string DetailDesc { get; private set; }

        private bool _isDetailVisible;
        public bool IsDetailVisible
        {
            get => _isDetailVisible;
            set => SetProperty(ref _isDetailVisible, value);
        }

        private bool _isListVisible;
        public bool IsListVisible
        {
            get => _isListVisible;
            set => SetProperty(ref _isListVisible, value);
        }
        public ObservableCollection<MediaEditModel> DetailItems { get; } = new();

        // Команды
        public DelegateCommand<PersonalListEditModel> OpenDetailCommand { get; }
        public DelegateCommand CloseDetailCommand { get; }
        public DelegateCommand CreateListCommand { get; }
        public DelegateCommand<PersonalListEditModel> EditListCommand { get; }
        public DelegateCommand<PersonalListEditModel> DeleteListCommand { get; }
        public DelegateCommand ManageItemsCommand { get; }
        public DelegateCommand<MediaEditModel> RemoveItemCommand { get; }


        public ListScreenViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            // Инициализация команд
            OpenDetailCommand = new DelegateCommand<PersonalListEditModel>(OpenDetail);
            CloseDetailCommand = new DelegateCommand(CloseDetail);
            CreateListCommand = new DelegateCommand(CreateList);
            EditListCommand = new DelegateCommand<PersonalListEditModel>(EditList);
            DeleteListCommand = new DelegateCommand<PersonalListEditModel>(DeleteList);
            ManageItemsCommand = new DelegateCommand(ManageItems);
            RemoveItemCommand = new DelegateCommand<MediaEditModel>(RemoveItem);
        }

        private void OpenDetail(PersonalListEditModel list)
        {
            SelectedList = list;
            DetailName = list.Name;
            DetailDesc = list.Description;
            RefreshDetailItems();
        }

        private void CloseDetail()
        {
            SelectedList = null;
        }

        private void RefreshDetailItems()
        {
            if (SelectedList == null || AllItems == null) return;

            var itemsInList = AllItems
                .Where(item => SelectedList.Items.Contains(item))
                .ToList();

            DetailItems.Clear();
            foreach (var item in itemsInList)
                DetailItems.Add(item);
        }

        private async void CreateList()
        {
            var result = await _dialogService.ShowDialogAsync("ListEditorView", new DialogParameters());

            if (result.Result == ButtonResult.OK)
            {
                var newList = result.Parameters.GetValue<PersonalListEditModel>("result");
                if (newList != null)
                {
                    Lists.Add(new PersonalListEditModel
                    {
                        Id = newList.Id,
                        Name = newList.Name,
                        Description = newList.Description,
                        Type = newList.Type,
                        Items = newList.Items
                    });
                }
            }
        }

        private async void EditList(PersonalListEditModel list)
        {
            var itemIds = new HashSet<Guid>(list.Items.Select(item => item.Id));
            var editModel = new PersonalListEditModel
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                Type = list.Type,
                Items = new ObservableCollection<MediaEditModel>(AllItems.Where(i => itemIds.Contains(i.Id)).Select(i => new MediaEditModel { Id = i.Id, Title = i.Title }))
            };

            // Передаём модель через параметры диалога
            var parameters = new DialogParameters
            {
                {"existingList", editModel}
            };

            var result = await _dialogService.ShowDialogAsync("ListEditorView", parameters);

            if (result.Result == ButtonResult.OK)
            {
                var updated = result.Parameters.GetValue<PersonalListEditModel>("result");
                list.Name = updated.Name;
                list.Description = updated.Description;
                list.Type = updated.Type;
                list.Items = updated.Items;
            }
        }

        private void DeleteList(PersonalListEditModel list)
        {
            if (System.Windows.MessageBox.Show(
                $"Delete list '{list.Name}'?", "Confirm",
                System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                Lists.Remove(list);
            }
        }

        private async void ManageItems()
        {
            if (SelectedList == null)
                return;

            // 1. Подготовка параметров для диалога
            var parameters = new DialogParameters
            {
                {"allItems", AllItems},
                {"currentIds", SelectedList.ItemIds.ToList()}
            };

            // 2. Открытие диалога и ожидание результата
            var result = await _dialogService.ShowDialogAsync("ListItemsView", parameters);

            if (result.Result != ButtonResult.OK)
                return;

            // 3. Получение выбранных ID из результата
            var selectedIds = result.Parameters.GetValue<List<Guid>>("selectedIds");
            if (selectedIds == null || !selectedIds.Any())
            {
                SelectedList.Items = new ObservableCollection<MediaEditModel>(); // очищаем список
                RefreshDetailItems();
                return;
            }

            // 4. Оптимизированный поиск соответствующих MediaEditModel
            var idSet = new HashSet<Guid>(selectedIds); // O(1) поиск
            var selectedItems = AllItems
                .Where(item => idSet.Contains(item.Id))
                .Select(item => new MediaEditModel
                {
                    Id = item.Id,
                    Title = item.Title
                    // Добавьте другие поля, если нужно
                })
                .ToList();

            // 5. Обновление коллекции
            SelectedList.Items = new ObservableCollection<MediaEditModel>(selectedItems);
            RefreshDetailItems();
        }

        private void RemoveItem(MediaEditModel item)
        {
            SelectedList.Items.Remove(item);
            RefreshDetailItems();
        }
    }
}
