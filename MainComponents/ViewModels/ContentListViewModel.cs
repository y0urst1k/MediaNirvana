using System.Collections.ObjectModel;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.Interface;
using Infrastructure.Service;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class ContentListViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private ObservableCollection<ContentCardViewModel> _cards;
        public ObservableCollection<ContentCardViewModel> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        public ContentListViewModel(IEventAggregator eventAggregator)
        {
            Cards = new ObservableCollection<ContentCardViewModel>();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<DeleteMediaRequestedEvent>()
            .Subscribe(OnMediaDeleted);
        }

        // Метод для загрузки данных
        public async Task LoadItemsAsync(
            IEnumerable<MediaItem> items,
            Guid userId, // нужен для получения DTO
            MediaEditService mediaEditService, // сервис для получения DTO
            IEventAggregator eventAggregator,
            IDialogService dialogService,
            IEditContentDialogService editContentDialogService,
            CancellationToken ct = default)
        {
            Cards.Clear();

            foreach (var item in items)
            {
                // Получаем DTO для конкретного MediaItem
                var dto = await mediaEditService.GetEditModelAsync(item.Id, userId, ct);

                // Передаем DTO в CardViewModel
                Cards.Add(new ContentCardViewModel(
                    dto,
                    eventAggregator,
                    dialogService,
                    editContentDialogService));
            }
        }

        private void OnMediaDeleted(Guid deletedId)
        {
            var cardToRemove = _cards.FirstOrDefault(card => card.Item.Id == deletedId);
            if (cardToRemove != null)
            {
                _cards.Remove(cardToRemove);
            }
        }
    }
}