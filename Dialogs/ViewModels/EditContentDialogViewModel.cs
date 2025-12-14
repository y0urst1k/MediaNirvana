using Infrastructure.DTO;
using Infrastructure.EF.Enum;

namespace Dialogs.ViewModels
{
    public class EditContentDialogViewModel : BindableBase, IDialogAware
    {
        private MediaEditModel _originalItem;
        private MediaEditModel _editedItem;

        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public int? Year { get; set; }
        public string Country { get; set; }
        public MediaType Type { get; set; }
        public InteractionStatus Status { get; set; }
        public int? DurationMinutes { get; set; }
        public int? SeasonsCount { get; set; }
        public int? EpisodesCount { get; set; }
        public string OfficialRating { get; set; }
        public string Synopsis { get; set; }
        public string Notes { get; set; }
        public string TagsInput { get; set; }

        public IEnumerable<MediaType> TypeOptions => Enum.GetValues(typeof(MediaType)).Cast<MediaType>();
        public IEnumerable<InteractionStatus> StatusOptions => Enum.GetValues(typeof(InteractionStatus)).Cast<InteractionStatus>();

        // Свойства видимости для UI
        public bool IsVideoFieldsVisible { get; private set; }
        public bool IsShowFieldsVisible { get; private set; }

        // Команды
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public DialogCloseListener RequestClose { get; set; }

        public EditContentDialogViewModel()
        {
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CancelCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        public string FormTitle => "Edit Content";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() {}

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _originalItem = parameters.GetValue<MediaEditModel>("item");

            if (_originalItem == null)
                throw new ArgumentNullException(nameof(_originalItem), "Параметр 'item' не передан в диалог");

            // Заполняем поля VM из исходного объекта
            Title = _originalItem.Title;
            OriginalTitle = _originalItem.OriginalTitle;
            Year = _originalItem.Year;
            Country = _originalItem.Country;
            Type = _originalItem.Type;
            Status = _originalItem.Status;
            DurationMinutes = _originalItem.DurationMinutes;
            SeasonsCount = _originalItem.SeasonsCount;  // Исправлено: SeasonsCount
            EpisodesCount = _originalItem.EpisodesCount; // Исправлено: EpisodesCount
            OfficialRating = _originalItem.OfficialRating;
            Synopsis = _originalItem.Synopsis;          // Исправлено: Synopsis
            Notes = _originalItem.UserNotes;

            // Обработка тегов: преобразуем список в строку через запятую
            var tags = _originalItem.TagsInput?.Split(',').Select(t => t.Trim()).Where(tag => !string.IsNullOrWhiteSpace(tag)) ?? Enumerable.Empty<string>();

            TagsInput = string.Join(", ", tags);

            // Обновляем видимость полей
            UpdateFieldVisibility();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title);
        }

        private void OnSave()
        {
            if (!CanSave())
            {
                // Можно использовать IDialogService для показа сообщения
                return;
            }

            // Создаём обновлённый объект
            _editedItem = new MediaEditModel
            {
                Title = Title,
                OriginalTitle = OriginalTitle,
                Year = Year,
                Country = Country,
                Type = Type,
                DurationMinutes = DurationMinutes,
                Status = Status,
                SeasonsCount = SeasonsCount,
                EpisodesCount = EpisodesCount,
                OfficialRating = OfficialRating,
                Synopsis = Synopsis,
                UserNotes = Notes,
                TagsInput = string.Join(", ",TagsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList() ?? new List<string>())
            };

            var parameters = new DialogParameters
            {
                { "updatedItem", _editedItem }
            };

            // Создаём результат диалога
            var result = new DialogResult(ButtonResult.OK)
            {
                Parameters = parameters
            };

            // Закрываем диалог и передаём результат
            RequestClose.Invoke(result);
        }

        private void UpdateFieldVisibility()
        {
            IsVideoFieldsVisible = Type == MediaType.Film || Type == MediaType.Series;
            IsShowFieldsVisible = Type == MediaType.Series;
        }
    }
}