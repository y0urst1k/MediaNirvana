using System.Collections.ObjectModel;
using Infrastructure.DTO;
using Infrastructure.Interface;
using Infrastructure.Service;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ContentListViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly MediaEditService _mediaEditService;
        private readonly IDialogService _dialogService;
        private readonly IEditContentDialogService _editContentDialogService;

        private ObservableCollection<ContentCardViewModel> _cards;
        public ObservableCollection<ContentCardViewModel> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        // Команды для взаимодействия с родительским VM
        public DelegateCommand<MediaEditModel> DeleteCommand { get; }
        public DelegateCommand<MediaEditModel> DetailCommand { get; }
        public DelegateCommand<MediaEditModel> EditCommand { get; }

        public ContentListViewModel(
            IEventAggregator eventAggregator,
            MediaEditService mediaEditService,
            IDialogService dialogService,
            IEditContentDialogService editContentDialogService)
        {
            _eventAggregator = eventAggregator;
            _mediaEditService = mediaEditService;
            _dialogService = dialogService;
            _editContentDialogService = editContentDialogService;

            Cards = new ObservableCollection<ContentCardViewModel>();

            // Инициализация команд
            DeleteCommand = new DelegateCommand<MediaEditModel>(OnDelete);
            DetailCommand = new DelegateCommand<MediaEditModel>(OnDetail);
            EditCommand = new DelegateCommand<MediaEditModel>(OnEdit);

            // Подписка на события удаления
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>()
                .Subscribe(OnItemDeleted);
            _eventAggregator.GetEvent<ItemUpdatedEvent>()
                .Subscribe(OnItemUpdated);
            _eventAggregator.GetEvent<MediaAddedEvent>()
                .Subscribe(OnItemCreated);
        }

        public async Task LoadItemsAsync(Guid userId, CancellationToken ct = default)
        {
            Cards.Clear();

            var mediaItems = await _mediaEditService.GetAllMediaItemsAsync(userId, ct);

            Cards.Clear();
            foreach (var item in mediaItems)
            {
                Cards.Add(new ContentCardViewModel(
                    item,
                    _eventAggregator,
                    _dialogService,
                    _editContentDialogService));
            }
        }

        private void OnDelete(MediaEditModel item) =>
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(item.Id);

        private void OnDetail(MediaEditModel item) =>
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(item);

        private void OnEdit(MediaEditModel item)
        {
            // Логика редактирования (например, открытие диалога)
            _editContentDialogService.ShowEditDialogAsync(item);
        }

        private void OnItemUpdated(MediaEditModel updatedItem)
        {
            var card = Cards.FirstOrDefault(c => c.Item.Id == updatedItem.Id);
            if (card != null) card.Item = updatedItem;
        }

        private void OnItemDeleted(Guid deletedId)
        {
            var card = Cards.FirstOrDefault(c => c.Item.Id == deletedId);
            if (card != null) Cards.Remove(card);
        }

        private void OnItemCreated(MediaEditModel newItem)
        {
            Cards.Add(new ContentCardViewModel(
                newItem,
                _eventAggregator,
                _dialogService,
                _editContentDialogService));
        }
    }
}