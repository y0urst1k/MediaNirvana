using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity
{
    public class UserLabel : Base.Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public LabelType Type { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<UserLabelLink> Links { get; set; } = new();
    }
}