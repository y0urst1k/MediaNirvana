using Infrastructure.EF.Entity.IndependentEntity;

namespace Infrastructure.EF.Entity.ConnectingEntity
{
    public class MediaItemCreator
    {
        public Guid MediaItemId { get; set; }
        public MediaItem MediaItem { get; set; } = null!;
        public Guid CreatorId { get; set; }
        public Creator Creator { get; set; } = null!;
        public string? Role { get; set; }
    }
}