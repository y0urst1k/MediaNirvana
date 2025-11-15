using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity.IndependentEntity
{
    public class MediaItem : Base.Entity
    {
        public MediaType Type { get; set; }
        public string Title { get; set; } = null!;
        public string? OriginalTitle { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public int? DurationMinutes { get; set; }
        public int? SeasonsCount { get; set; }
        public int? EpisodesCount { get; set; }
        public string? OfficialRating { get; set; }
        public string? Synopsis { get; set; }

        // Навигационые свойства
        public List<MediaInstance> Instances { get; set; } = new();
        public List<MediaItemGenre> MediaItemGenres { get; set; } = new();
        public List<MediaItemCreator> MediaItemCreators { get; set; } = new();
        public List<Relationship> RelationshipsSource { get; set; } = new();
        public List<Relationship> RelationshipsTarget { get; set; } = new();
    }
}