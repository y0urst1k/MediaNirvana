using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Infrastructure.DTO;
using Infrastructure.EF.Enum;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class MediaDetailViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private MediaEditModel _originalItem;

        // === Свойства для привязки ===
        private MediaEditModel _currentItem;
        public MediaEditModel CurrentItem
        {
            get => _currentItem;
            set => SetProperty(ref _currentItem, value);
        }

        private ObservableCollection<MediaEditModel> _allItems;
        public ObservableCollection<MediaEditModel> AllItems
        {
            get => _allItems;
            set => SetProperty(ref _allItems, value);
        }

        private ObservableCollection<MediaEditModel> _relatedItems;
        public ObservableCollection<MediaEditModel> RelatedItems
        {
            get => _relatedItems;
            set => SetProperty(ref _relatedItems, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }
        public IEnumerable<InteractionStatus> StatusValues =>
        Enum.GetValues(typeof(InteractionStatus)).Cast<InteractionStatus>();

        // === Команды ===
        public ICommand EditCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand BackCommand { get; }

        public MediaDetailViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            RelatedItems = new ObservableCollection<MediaEditModel>();
            AllItems = new ObservableCollection<MediaEditModel>();

            EditCommand = new DelegateCommand(() => IsEditing = true);
            CancelCommand = new DelegateCommand(CancelChanges);
            SaveCommand = new DelegateCommand(SaveChanges, () => IsEditing)
                .ObservesProperty(() => IsEditing);
            DeleteCommand = new DelegateCommand(DeleteItem);
            BackCommand = new DelegateCommand(GoBack);

            // Подписка на открытие детального просмотра
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Subscribe(OnNavigatedTo);
        }

        // === Логика ===
        private void OnNavigatedTo(MediaEditModel item)
        {
            if (item == null) return;

            // Сохраняем ссылку на оригинал (или копию из стора)
            _originalItem = item;

            // Создаем клон для работы в UI
            CurrentItem = (MediaEditModel)_originalItem.Clone();

            IsEditing = false;
            FindRelatedItems(CurrentItem);
        }

        private void FindRelatedItems(MediaEditModel current)
        {
            RelatedItems.Clear();
            if (AllItems == null || !AllItems.Any()) return;

            string firstWord = current.Title.Split(' ')[0];
            var related = AllItems
                .Where(i => i.Id != current.Id &&
                              (i.Title.Contains(firstWord) ||
                               current.Title.Contains(i.Title.Split(' ')[0])))
                .Take(3)
                .ToList();

            foreach (var item in related) RelatedItems.Add(item);
        }

        // === Обработчики команд ===
        private void CancelChanges()
        {
            // Просто заново клонируем оригинал, затирая изменения
            CurrentItem = (MediaEditModel)_originalItem.Clone();
            IsEditing = false;
        }

        private void SaveChanges()
        {
            // Переносим изменения из CurrentItem в _originalItem 
            // (Это нужно, если список родителя ссылается на _originalItem)
            ApplyChanges(_originalItem, CurrentItem);

            // Отправляем событие о сохранении в базу
            _eventAggregator.GetEvent<ItemUpdatedEvent>().Publish(_originalItem);

            IsEditing = false;
        }

        private void DeleteItem()
        {
            var result = MessageBox.Show($"Delete '{CurrentItem.Title}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(CurrentItem.Id);
                GoBack();
            }
        }

        private void ApplyChanges(MediaEditModel target, MediaEditModel source)
        {
            target.Status = source.Status;
            target.PersonalRating = source.PersonalRating;
            target.UserNotes = source.UserNotes;
            target.HasPhysicalCopy = source.HasPhysicalCopy;
            target.HasDigitalCopy = source.HasDigitalCopy;
            target.Format = source.Format;
            target.Source = source.Source;
            target.Location = source.Location;
            target.PurchasePrice = source.PurchasePrice;

        }

        private void GoBack()
        {
            // Возвращаемся на предыдущий экран
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(null); // null = закрыть детальный вид
        }
    }
}