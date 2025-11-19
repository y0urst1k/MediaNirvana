using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;

namespace Infrastructure.EF.Entity.ConnectingEntity
{
    public class UserInteractionTag
    {
        public Guid UserInteractionId { get; set; }
        public UserInteraction UserInteraction { get; set; } = null!;
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}