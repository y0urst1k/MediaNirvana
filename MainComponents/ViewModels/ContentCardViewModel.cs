using Infrastructure.DTO;
using Infrastructure.Interface;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ContentCardViewModel : BindableBase
    {
        private MediaEditModel _item;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly IEditContentDialogService _editContentDialogService;

        public MediaEditModel Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        // Команды Prism
        public DelegateCommand ViewDetailCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand EditCommand { get; }

        public ContentCardViewModel(
            MediaEditModel item,
            IEventAggregator eventAggregator,
            IDialogService dialogService,
            IEditContentDialogService editContentDialogService)
        {
            Item = item;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            ViewDetailCommand = new DelegateCommand(OnViewDetail);
            DeleteCommand = new DelegateCommand(OnDelete);
            EditCommand = new DelegateCommand(async () => await OnEdit());
        }

        private void OnViewDetail()
        {
            // Пример: публикуем событие о просмотре детализации
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(Item);
        }

        private void OnDelete()
        {
            var parameters = new DialogParameters
            {
                { "message", $"Delete '{Item.Title}'?" },
                { "title", "Confirm Deletion" }
            };

            _dialogService.ShowDialog(
                "MessageBoxDialog",
                parameters,
                result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(Item.Id);
                    }
                });
        }
        private async Task OnEdit()
        {
            var updatedItem = await _editContentDialogService.ShowEditDialogAsync(Item);
            if (updatedItem != null)
            {
                Item.Title = updatedItem.Title;
                Item.OriginalTitle = updatedItem.OriginalTitle;
                Item.Type = updatedItem.Type;
                Item.Status = updatedItem.Status;
                Item.Year = updatedItem.Year;
                Item.Country = updatedItem.Country;
                Item.DurationMinutes = updatedItem.DurationMinutes;
                Item.SeasonsCount = updatedItem.SeasonsCount;
                Item.EpisodesCount = updatedItem.EpisodesCount;
                Item.OfficialRating = updatedItem.OfficialRating;
                Item.Synopsis = updatedItem.Synopsis;
                Item.UserNotes = updatedItem.UserNotes;
                Item.TagsInput = updatedItem.TagsInput;
                _eventAggregator.GetEvent<ItemUpdatedEvent>().Publish(Item);
            }
        }
    }
}