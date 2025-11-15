using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity
{
    public class UserLabelLink : Base.Entity
    {
        public Guid UserLabelId { get; set; }
        public UserLabel UserLabel { get; set; } = null!;
        public LinkTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
    }
}