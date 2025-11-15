namespace Infrastructure.EF.Entity
{
    public class Tag : Base.Entity
    {
        public string Name { get; set; } = null!;
        public List<UserInteractionTag> UserInteractionTags { get; set; } = new();
    }
}