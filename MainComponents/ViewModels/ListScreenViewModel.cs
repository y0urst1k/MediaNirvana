using System.Collections.ObjectModel;
using Infrastructure.DTO;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ListScreenViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        // Данные
        public ObservableCollection<PersonalListEditModel> Lists { get; } = new();
        public ObservableCollection<MediaEditModel> AllItems { get; private set; } = new();
        public ObservableCollection<MediaEditModel> DetailItems { get; } = new();

        // Текущно выбранный список
        private PersonalListEditModel _selectedList;
        public PersonalListEditModel SelectedList
        {
            get => _selectedList;
            set
            {
                if (SetProperty(ref _selectedList, value))
                {
                    // Автоматическое переключение видимости при выборе списка
                    IsListVisible = _selectedList == null;
                    IsDetailVisible = _selectedList != null;

                    if (_selectedList != null)
                    {
                        RefreshDetailItems();
                    }
                }
            }
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
            OpenDetailCommand = new DelegateCommand<PersonalListEditModel>(list => SelectedList = list);
            CloseDetailCommand = new DelegateCommand(() => SelectedList = null);

            CreateListCommand = new DelegateCommand(CreateList);
            EditListCommand = new DelegateCommand<PersonalListEditModel>(EditList);
            DeleteListCommand = new DelegateCommand<PersonalListEditModel>(DeleteList);
            ManageItemsCommand = new DelegateCommand(ManageItems);
            RemoveItemCommand = new DelegateCommand<MediaEditModel>(RemoveItem);

            _eventAggregator.GetEvent<MediaLibraryLoadedEvent>().Subscribe(OnLibraryLoaded);
        }

        private void OnLibraryLoaded(ObservableCollection<MediaEditModel> items)
        {
            AllItems.Clear();
            AllItems.AddRange(items);
        }

        private void RefreshDetailItems()
        {
            DetailItems.Clear();
            if (SelectedList == null || AllItems == null) return;

            // Находим полные объекты MediaEditModel на основе ID, сохраненных в списке
            // (Предполагаем, что SelectedList.Items содержит хотя бы ID корректно)
            // Но лучше полагаться на поиск в AllItems по ID

            var ids = SelectedList.Items.Select(x => x.Id).ToHashSet();

            var itemsToShow = AllItems.Where(x => ids.Contains(x.Id)).ToList();

            DetailItems.AddRange(itemsToShow);
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
            if (SelectedList == null) return;

            // Передаем AllItems и список ID текущих элементов
            var currentIds = SelectedList.Items.Select(x => x.Id).ToList();
            var parameters = new DialogParameters
            {
                {"allItems", AllItems},
                {"currentIds", currentIds}
            };

            var result = await _dialogService.ShowDialogAsync("ListItemsView", parameters);

            if (result.Result == ButtonResult.OK)
            {
                var selectedIds = result.Parameters.GetValue<List<Guid>>("selectedIds");

                // Обновляем SelectedList.Items на основе выбранных ID
                SelectedList.Items.Clear();
                if (selectedIds != null)
                {
                    var newItems = AllItems.Where(x => selectedIds.Contains(x.Id)).ToList();
                    SelectedList.Items.AddRange(newItems);
                }

                RefreshDetailItems();
            }
        }

        private void RemoveItem(MediaEditModel item)
        {
            if (SelectedList == null) return;

            // Удаляем по ID, чтобы точно найти нужный объект
            var itemInList = SelectedList.Items.FirstOrDefault(x => x.Id == item.Id);
            if (itemInList != null)
            {
                SelectedList.Items.Remove(itemInList);
                RefreshDetailItems();
            }
        }
    }
}
