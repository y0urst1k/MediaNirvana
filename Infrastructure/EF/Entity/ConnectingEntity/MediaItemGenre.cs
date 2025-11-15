using Infrastructure.EF.Entity.IndependentEntity;

namespace Infrastructure.EF.Entity.ConnectingEntity
{
    public class MediaItemGenre : Base.Entity
    {
        public Guid MediaItemId { get; set; }
        public MediaItem MediaItem { get; set; } = null!;
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}