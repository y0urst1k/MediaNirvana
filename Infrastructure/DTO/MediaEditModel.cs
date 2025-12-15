using Infrastructure.EF.Enum;

namespace Infrastructure.DTO
{
    public class MediaEditModel : BindableBase, ICloneable
    {
        public Guid Id { get; set; }

        // Поля из MediaItem
        public string Title { get; set; } = null!;
        public string? OriginalTitle { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public MediaType Type { get; set; }
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
        public bool HasPhysicalCopy { get; set; }
        public bool HasDigitalCopy { get; set; }

        public double PurchasePrice { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}