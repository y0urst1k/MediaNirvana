using Infrastructure.EF.Entity.ConnectingEntity;

namespace Infrastructure.EF.Entity.IndependentEntity
{
    public class Tag : Base.Entity
    {
        public string Name { get; set; } = null!;
        public List<UserInteractionTag> UserInteractionTags { get; set; } = new();
    }
}