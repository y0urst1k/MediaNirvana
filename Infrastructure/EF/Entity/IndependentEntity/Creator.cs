using Infrastructure.EF.Entity.ConnectingEntity;

namespace Infrastructure.EF.Entity.IndependentEntity
{
    public class Creator : Base.Entity
    {
        public string Name { get; set; } = null!;
        public List<MediaItemCreator> MediaItemCreators { get; set; } = new();
    }
}