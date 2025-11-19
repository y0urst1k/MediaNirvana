using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.EF.Enum;

namespace Infrastructure.EF.Entity.ConnectingEntity
{
    public class UserLabelLink : Base.Entity
    {
        public Guid UserLabelId { get; set; }
        public UserLabel UserLabel { get; set; } = null!;
        public LinkTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
    }
}