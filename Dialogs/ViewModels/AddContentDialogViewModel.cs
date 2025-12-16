using System.ComponentModel;
using Infrastructure.DTO;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Enum;
using Infrastructure.Interface;
using Infrastructure.Service;

namespace Dialogs.ViewModels
{
    public class AddContentDialogViewModel : BindableBase, IDialogAware
    {
        private readonly MediaEditService _mediaEditService;
        private readonly ISessionService _sessionService;

        // === Данные формы ===
        private MediaEditModel _model;
        public MediaEditModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        // === Видимость секций ===
        private bool _showVideoFields;
        public bool ShowVideoFields
        {
            get => _showVideoFields;
            private set => SetProperty(ref _showVideoFields, value);
        }

        private bool _showShowFields;
        public bool ShowShowFields
        {
            get => _showShowFields;
            private set => SetProperty(ref _showShowFields, value);
        }

        private bool _showInventoryDetails;
        public bool ShowInventoryDetails
        {
            get => _showInventoryDetails;
            private set => SetProperty(ref _showInventoryDetails, value);
        }

        public IEnumerable<MediaType> MediaTypes => Enum.GetValues(typeof(MediaType)).Cast<MediaType>();
        public IEnumerable<InteractionStatus> InteractionStatuses => Enum.GetValues(typeof(InteractionStatus)).Cast<InteractionStatus>();

        // === Команды ===
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        // === IDialogAware ===
        public string Title => "Добавить контент";

        public DialogCloseListener RequestClose { get; set; }

        public AddContentDialogViewModel(MediaEditService mediaEditService, ISessionService sessionService)
        {
            _mediaEditService = mediaEditService;
            _sessionService = sessionService;

            Model = new MediaEditModel
            {
                StartDate = DateTimeOffset.Now,
                Type = MediaType.Film
            };

            // Команды
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CancelCommand = new DelegateCommand(OnCancel);

            // Подписываемся на изменения Model
            Model.PropertyChanged += OnModelPropertyChanged;

            UpdateVisibility();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();

            switch (e.PropertyName)
            {
                case nameof(MediaEditModel.Type):
                    UpdateVisibility();
                    break;
                case nameof(MediaEditModel.HasPhysicalCopy):
                case nameof(MediaEditModel.HasDigitalCopy):
                    UpdateInventoryVisibility();
                    break;
            }
        }

        private void UpdateVisibility()
        {
            var type = Model.Type;
            ShowVideoFields = type == MediaType.Film || type == MediaType.Series;
            ShowShowFields = type == MediaType.Series;
        }

        private void UpdateInventoryVisibility()
        {
            ShowInventoryDetails = Model.HasPhysicalCopy || Model.HasDigitalCopy;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Model.Title) && !string.IsNullOrWhiteSpace(Model.Type.ToString()); ;
        }

        private async void OnSave()
        {
            User user = _sessionService.CurrentUser;

            try
            {
                await _mediaEditService.CreateAsync(user.Id, Model);

                // Формируем результат диалога
                var parameters = new DialogParameters
                {
                    { "result", Model }
                };

                // Создаём результат диалога
                var result = new DialogResult(ButtonResult.OK)
                {
                    Parameters = parameters
                };

                RequestClose.Invoke(result);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Ошибка сохранения: {ex.Message}",
                    "Ошибка",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void OnCancel()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        // IDialogAware: вызывается при открытии диалога
        public void OnDialogOpened(IDialogParameters parameters)
        {
            var existingModel = parameters.GetValue<MediaEditModel>("model");

            if (existingModel != null)
            {
                // Отписываемся от старой
                Model.PropertyChanged -= OnModelPropertyChanged;

                // Присваиваем новую (лучше клонировать, если это редактирование)
                Model = existingModel;

                // Подписываемся на новую
                Model.PropertyChanged += OnModelPropertyChanged;
            }
            // Если null, оставляем ту, что создали в конструкторе

            UpdateVisibility();
            UpdateInventoryVisibility();
            SaveCommand.RaiseCanExecuteChanged();
        }

        // IDialogAware: можно ли закрыть диалог?
        public bool CanCloseDialog()
        {
            return true; // Всегда можно закрыть
        }

        // IDialogAware: вызывается при закрытии диалога
        public void OnDialogClosed()
        {
            // Очистка ресурсов, отписка от событий и т.п.
            Model.PropertyChanged -= OnModelPropertyChanged;
        }
    }
}