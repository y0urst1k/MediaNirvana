using System.Collections.ObjectModel;
using System.Windows;
using Infrastructure.DTO;
using Infrastructure.EF.Enum;
using Infrastructure.Interface;
using Infrastructure.Service;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ISessionService _sessionService;
        private readonly MediaEditService _mediaEditService;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        // === Данные приложения ===
        public ObservableCollection<MediaEditModel> MediaCollection { get; } = new();
        public ObservableCollection<PersonalListEditModel> Lists { get; } = new();

        public Array StatusOptions => Enum.GetValues(typeof(InteractionStatus));

        public List<LabelType> AvailableListTypes { get; } = new()
        {
            LabelType.List,
            LabelType.Collection,
            LabelType.Priority
        };

        // === Состояние UI ===
        private string _currentUsername;
        public string CurrentUsername
        {
            get => _currentUsername;
            set => SetProperty(ref _currentUsername, value);
        }

        private MediaEditModel _selectedItem;
        public MediaEditModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private PersonalListEditModel _selectedList;
        public PersonalListEditModel SelectedList
        {
            get => _selectedList;
            set => SetProperty(ref _selectedList, value);
        }

        private bool _isLoginVisible = true;
        public bool IsLoginVisible
        {
            get => _isLoginVisible;
            set => SetProperty(ref _isLoginVisible, value);
        }

        private bool _isMainAppVisible;
        public bool IsMainAppVisible
        {
            get => _isMainAppVisible;
            set => SetProperty(ref _isMainAppVisible, value);
        }

        private string _activeScreen = "Library";
        public string ActiveScreen
        {
            get => _activeScreen;
            set => SetProperty(ref _activeScreen, value);
        }

        // === Команды ===
        public DelegateCommand NavigateToLibraryCommand { get; }
        public DelegateCommand NavigateToListCommand { get; }
        public DelegateCommand<MediaEditModel> ShowDetailCommand { get; }
        public DelegateCommand BackToLibraryCommand { get; }
        public DelegateCommand DeleteItemCommand { get; }
        public DelegateCommand AddItemCommand { get; }
        public DelegateCommand<PersonalListEditModel> SelectListCommand { get; }
        public DelegateCommand<MediaEditModel> AddMediaToListCommand { get; }
        public DelegateCommand<MediaEditModel> RemoveMediaFromListCommand { get; }

        public MainWindowViewModel(ISessionService sessionService, MediaEditService mediaEditService, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _sessionService = sessionService;
            _mediaEditService = mediaEditService;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            // Инициализация команд
            NavigateToLibraryCommand = new DelegateCommand(() => SwitchScreen("Library"));
            NavigateToListCommand = new DelegateCommand(() => SwitchScreen("Lists"));
            ShowDetailCommand = new DelegateCommand<MediaEditModel>(ShowDetail);
            BackToLibraryCommand = new DelegateCommand(BackToLibrary);
            DeleteItemCommand = new DelegateCommand(DeleteItem);
            AddItemCommand = new DelegateCommand(AddItem);

            SelectListCommand = new DelegateCommand<PersonalListEditModel>(SelectList);
            AddMediaToListCommand = new DelegateCommand<MediaEditModel>(AddMediaToList);
            RemoveMediaFromListCommand = new DelegateCommand<MediaEditModel>(RemoveMediaFromList);

            SubscribeToEvents();

            // Изначально показываем экран входа
            IsLoginVisible = true;
            IsMainAppVisible = false;
        }

        // === Обработчики событий ===
        private async void OnLoginSuccess(string username)
        {
            CurrentUsername = username;

            // 1. Загружаем данные из сервиса
            var items = await _mediaEditService.GetAllMediaItemsAsync(_sessionService.CurrentUser.Id);
            MediaCollection.Clear();
            MediaCollection.AddRange(items);

            // 2. Рассылаем данные всем подписчикам (Sidebar, Detail, Tracker)
            _eventAggregator.GetEvent<MediaLibraryLoadedEvent>().Publish(MediaCollection);

            IsLoginVisible = false;
            IsMainAppVisible = true;
            SwitchScreen("Library");
        }

        private async Task OnDeleteMediaRequested(Guid mediaId)
        {
            await _mediaEditService.DeleteAsync(mediaId, _sessionService.CurrentUser.Id);
            var item = MediaCollection.FirstOrDefault(i => i.Id == mediaId);
            if (item != null)
            {
                MediaCollection.Remove(item);
                if (SelectedItem?.Id == mediaId)
                {
                    BackToLibrary();
                }
            }
        }

        private async Task OnItemUpdated(MediaEditModel updatedItem)
        {
            await _mediaEditService.UpdateAsync(updatedItem.Id, _sessionService.CurrentUser.Id, updatedItem);
            var existing = MediaCollection.FirstOrDefault(i => i.Id == updatedItem.Id);
            if (existing != null)
            {
                // Копируем все поля (можно оптимизировать через отражение или AutoMapper)
                existing.Title = updatedItem.Title;
                existing.OriginalTitle = updatedItem.OriginalTitle;
                existing.Year = updatedItem.Year;
                existing.Country = updatedItem.Country;
                existing.Type = updatedItem.Type;
                existing.DurationMinutes = updatedItem.DurationMinutes;
                existing.SeasonsCount = updatedItem.SeasonsCount;
                existing.EpisodesCount = updatedItem.EpisodesCount;
                existing.OfficialRating = updatedItem.OfficialRating;
                existing.Synopsis = updatedItem.Synopsis;
                existing.Status = updatedItem.Status;
                existing.StartDate = updatedItem.StartDate;
                existing.CompletionDate = updatedItem.CompletionDate;
                existing.PersonalRating = updatedItem.PersonalRating;
                existing.UserNotes = updatedItem.UserNotes;
                existing.TagsInput = updatedItem.TagsInput;
                existing.Format = updatedItem.Format;
                existing.Source = updatedItem.Source;
                existing.AcquisitionDate = updatedItem.AcquisitionDate;
                existing.Location = updatedItem.Location;
            }
        }

        private void OnViewMediaDetailRequested(MediaEditModel model)
        {
            if (model != null)
            {
                SelectedItem = model;
                SwitchScreen("Detail");
            }
            else
            {
                // Если пришел null — это сигнал "Назад"
                BackToLibrary();
            }
        }

        // === Бизнес-логика ===
        private void SubscribeToEvents()
        {
            // Навигация (Library, Lists)
            _eventAggregator.GetEvent<NavigateToEvent>().Subscribe(screenName =>
            {
                if (screenName == "ContentTracker") SwitchScreen("Library");
                else if (screenName == "ListScreen") SwitchScreen("Lists");
                else SwitchScreen(screenName);
            });

            // Открытие/Закрытие деталей (ОДНА подписка)
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Subscribe(OnViewMediaDetailRequested);

            // Остальные события
            _eventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(OnLoginSuccess);

            // Для async методов внутри Subscribe лучше использовать такую конструкцию, 
            // чтобы исключения не терялись, хотя ваш вариант тоже сработает.
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Subscribe(async id => await OnDeleteMediaRequested(id));
            _eventAggregator.GetEvent<ItemUpdatedEvent>().Subscribe(async item => await OnItemUpdated(item));

            _eventAggregator.GetEvent<AddMediaRequestedEvent>().Subscribe(OnAddMediaRequested);
        }

        private void SwitchScreen(string screenName)
        {
            ActiveScreen = screenName;
        }

        private void ShowDetail(MediaEditModel item)
        {
            SelectedItem = item;
            SwitchScreen("Detail");
        }

        private void BackToLibrary()
        {
            SelectedItem = null;
            SwitchScreen("Library");
        }

        private void DeleteItem()
        {
            if (SelectedItem == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedItem.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                MediaCollection.Remove(SelectedItem);
                BackToLibrary();
            }
        }

        private void AddItem()
        {
            _eventAggregator.GetEvent<AddMediaRequestedEvent>().Publish();
        }

        private void SelectList(PersonalListEditModel list)
        {
            SelectedList = list;
            SwitchScreen("Lists");
        }

        private void AddMediaToList(MediaEditModel media)
        {
            if (SelectedList == null || SelectedList.Items.Any(m => m.Id == media.Id))
                return;

            SelectedList.Items.Add(media);
        }

        private void RemoveMediaFromList(MediaEditModel media)
        {
            if (SelectedList == null)
                return;

            var item = SelectedList.Items.FirstOrDefault(m => m.Id == media.Id);
            if (item != null)
            {
                SelectedList.Items.Remove(item);
            }
        }

        private async void OnAddMediaRequested()
        {
            var parameters = new DialogParameters();
            var result = await _dialogService.ShowDialogAsync("AddContentDialog", parameters);

            if (result.Result == ButtonResult.OK)
            {
                var newItem = result.Parameters.GetValue<MediaEditModel>("result");
                if (newItem != null)
                {
                    await _mediaEditService.CreateAsync(_sessionService.CurrentUser.Id, newItem);
                    MediaCollection.Insert(0, newItem);
                    SelectedItem = newItem;
                    SwitchScreen("Detail");
                    _eventAggregator.GetEvent<MediaAddedEvent>().Publish(SelectedItem);
                }
            }
        }
    }
}
