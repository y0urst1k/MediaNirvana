using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity.IndependentEntity.DependentEntity
{
    public class UserInteraction : Base.Entity
    {
        public Guid MediaInstanceId { get; set; }
        public MediaInstance MediaInstance { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public InteractionStatus Status { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
        public decimal? PersonalRating { get; set; } // от 0-10
        public string? Notes { get; set; }
        public List<UserInteractionTag> Tags { get; set; } = new();
    }
}