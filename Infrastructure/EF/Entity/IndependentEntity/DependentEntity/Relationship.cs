using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity.IndependentEntity.DependentEntity
{
    public class Relationship : Base.Entity
    {
        public RelationshipType Type { get; set; }
        public Guid SourceMediaId { get; set; }
        public MediaItem SourceMedia { get; set; } = null!;
        public Guid TargetMediaId { get; set; }
        public MediaItem TargetMedia { get; set; } = null!;
        public string? RelationDescription { get; set; }
    }
}