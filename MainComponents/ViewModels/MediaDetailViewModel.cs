using System.Collections.ObjectModel;
using System.Windows.Input;
using Infrastructure.DTO;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class MediaDetailViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

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

            EditCommand = new DelegateCommand(Edit);
            CancelCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(Save, CanSave)
                .ObservesProperty(() => IsEditing)
                .ObservesProperty(() => CurrentItem);
            DeleteCommand = new DelegateCommand(Delete);
            BackCommand = new DelegateCommand(Back);

            _eventAggregator.GetEvent<ViewMediaDetailEvent>()
            .Subscribe(OnViewMediaDetailRequested);
        }

        // === Логика ===
        public void LoadItem(MediaEditModel item)
        {
            CurrentItem = item;
            IsEditing = false;
            FindRelatedItems(item);
        }

        private void FindRelatedItems(MediaEditModel current)
        {
            RelatedItems.Clear();
            if (AllItems == null) return;

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
        private void Edit() => IsEditing = true;

        private void Cancel() => LoadItem(CurrentItem);


        private bool CanSave() => IsEditing && CurrentItem != null;

        private void Save()
        {
            // 1. Публикуем событие обновления
            _eventAggregator.GetEvent<ItemUpdatedEvent>().Publish(CurrentItem);


            // 2. Дополнительно: можно уведомить о завершении редактирования
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(CurrentItem); // Повторная публикация


            IsEditing = false;
        }

        private void Delete()
        {
            var result = System.Windows.MessageBox.Show(
                $"Delete '{CurrentItem.Title}'?", "Confirm",
                System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                // Публикуем событие удаления с ID
                _eventAggregator.GetEvent<DeleteMediaRequestedEvent>().Publish(CurrentItem.Id);
            }
        }

        private void Back()
        {
            // Возвращаемся на предыдущий экран
            _eventAggregator.GetEvent<ViewMediaDetailEvent>().Publish(null); // null = закрыть детальный вид
        }

        private void OnViewMediaDetailRequested(MediaEditModel item)
        {
            if (item == null)
            {
                // Закрываем детальный экран (например, возвращаемся в библиотеку)
                // Можно опубликовать другое событие или использовать INavigationService
                return;
            }

            // Загружаем выбранный item
            LoadItem(item);
        }
    }
}