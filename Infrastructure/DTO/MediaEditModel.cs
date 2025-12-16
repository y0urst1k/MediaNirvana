using Infrastructure.EF.Enum;

namespace Infrastructure.DTO
{
    public class MediaEditModel : BindableBase, ICloneable
    {
        public Guid Id { get; set; }

        // Поля из MediaItem

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public string? OriginalTitle { get; set; }

        public int? Year { get; set; }
        public string? Country { get; set; }
        public MediaType _type;
        public MediaType Type { get => _type; set => SetProperty(ref _type, value); }
        public int? DurationMinutes { get; set; }
        public int? SeasonsCount { get; set; }
        public int? EpisodesCount { get; set; }
        public string? OfficialRating { get; set; }
        public string? Synopsis { get; set; }

        // Поля из UserInteraction (предполагаем, что редактируем для текущего пользователя)
        public InteractionStatus Status { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
        public decimal? PersonalRating { get; set; }
        public string? UserNotes { get; set; } // переименовали, чтобы не путать с MediaItem.Notes

        // Теги как строка (для удобного редактирования)
        public string TagsInput { get; set; } = "";

        // Дополнительные поля из MediaInstance (если нужны)
        public MediaFormat? Format { get; set; }
        public MediaSource? Source { get; set; }
        public DateTimeOffset? AcquisitionDate { get; set; }
        public string? Location { get; set; }
        public bool _hasPhysicalCopy;
        public bool HasPhysicalCopy { get => _hasPhysicalCopy; set => SetProperty(ref _hasPhysicalCopy, value); }
        public bool _hasDigitalCopy;
        public bool HasDigitalCopy { get => _hasDigitalCopy; set => SetProperty(ref _hasDigitalCopy, value); }

        public double PurchasePrice { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}