using Infrastructure.EF.Entity.ConnectingEntity;

namespace Infrastructure.EF.Entity.IndependentEntity
{
    public class Genre : Base.Entity
    {
        public string Name { get; set; } = null!;
        public List<MediaItemGenre> MediaItemGenres { get; set; } = new();
    }
}