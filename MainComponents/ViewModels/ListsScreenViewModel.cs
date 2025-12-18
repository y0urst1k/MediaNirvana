using System.Collections.ObjectModel;
using System.Windows;
using Infrastructure.DTO;
using Infrastructure.Interface;
using Infrastructure.Service;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ListsScreenViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly PersonalListEditModelService _listEditService;
        private readonly ISessionService _sessionService;

        // Данные
        public ObservableCollection<PersonalListEditModel> Lists { get; private set; } = new();
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

        private bool _isListVisible = true;
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


        public ListsScreenViewModel(IDialogService dialogService, IEventAggregator eventAggregator, PersonalListEditModelService listEditService, ISessionService sessionService)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _listEditService = listEditService;
            _sessionService = sessionService;

            // Инициализация команд
            OpenDetailCommand = new DelegateCommand<PersonalListEditModel>(list => SelectedList = list);
            CloseDetailCommand = new DelegateCommand(() => SelectedList = null);

            CreateListCommand = new DelegateCommand(CreateList);
            EditListCommand = new DelegateCommand<PersonalListEditModel>(EditList);
            DeleteListCommand = new DelegateCommand<PersonalListEditModel>(DeleteList);
            ManageItemsCommand = new DelegateCommand(ManageItems);
            RemoveItemCommand = new DelegateCommand<MediaEditModel>(RemoveItem);

            _eventAggregator.GetEvent<MediaLibraryLoadedEvent>().Subscribe(items =>
            {
                AllItems.Clear();
                AllItems.AddRange(items);
            });

            _eventAggregator.GetEvent<ListsLoadedEvent>().Subscribe(lists =>
            {
                Lists.Clear();
                Lists.AddRange(lists); // Получаем ссылку на общую коллекцию
            });
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
                    // 1. Сохраняем в БД через сервис
                    await _listEditService.CreateListAsync(_sessionService.CurrentUser.Id, newList);

                    // 2. Добавляем в UI
                    Lists.Add(newList);
                }
            }
        }

        private async void EditList(PersonalListEditModel list)
        {
            // Клонируем для редактирования (чтобы при отмене не поменялось в UI)
            var editModel = new PersonalListEditModel
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                Type = list.Type,
                Items = new ObservableCollection<MediaEditModel>(list.Items)
            };

            var parameters = new DialogParameters { { "existingList", editModel } };
            var result = await _dialogService.ShowDialogAsync("ListEditorView", parameters);

            if (result.Result == ButtonResult.OK)
            {
                var updated = result.Parameters.GetValue<PersonalListEditModel>("result");

                // 1. Обновляем DTO (поля)
                list.Name = updated.Name;
                list.Description = updated.Description;
                list.Type = updated.Type;
                // Items здесь не меняем, это делается через ManageItems

                // 2. Сохраняем в БД
                await _listEditService.UpdateListAsync(list);
            }
        }

        private async void DeleteList(PersonalListEditModel list)
        {
            if (MessageBox.Show($"Delete list '{list.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // 1. Удаляем из БД
                await _listEditService.DeleteListAsync(list.Id);
                // 2. Удаляем из UI
                Lists.Remove(list);
                SelectedList = null;
            }
        }

        private async void ManageItems()
        {
            if (SelectedList == null) return;

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

                // 1. Обновляем коллекцию в UI
                SelectedList.Items.Clear();
                if (selectedIds != null)
                {
                    var newItems = AllItems.Where(x => selectedIds.Contains(x.Id)).ToList();
                    SelectedList.Items.AddRange(newItems);
                }

                // 2. Сохраняем изменения в БД
                await _listEditService.UpdateListAsync(SelectedList);

                RefreshDetailItems();
            }
        }

        private async void RemoveItem(MediaEditModel item)
        {
            if (SelectedList == null) return;

            var itemInList = SelectedList.Items.FirstOrDefault(x => x.Id == item.Id);
            if (itemInList != null)
            {
                SelectedList.Items.Remove(itemInList);

                // Сохраняем изменения в БД сразу
                await _listEditService.UpdateListAsync(SelectedList);

                RefreshDetailItems();
            }
        }
    }
}
