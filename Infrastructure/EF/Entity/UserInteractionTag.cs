namespace Infrastructure.EF.Entity
{
    public class UserInteractionTag
    {
        public Guid UserInteractionId { get; set; }
        public UserInteraction UserInteraction { get; set; } = null!;
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}