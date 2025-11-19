using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity.IndependentEntity.DependentEntity
{
    public class MediaInstance : Base.Entity
    {
        public Guid MediaItemId { get; set; }
        public MediaItem MediaItem { get; set; } = null!;
        public MediaFormat Format { get; set; }
        public MediaSource Source { get; set; }
        public DateTimeOffset? AcquisitionDate { get; set; }
        public string? Location { get; set; }
        public string? SerialNumber { get; set; }
        public string? Notes { get; set; }

        public List<UserInteraction> Interactions { get; set; } = new();
    }
}