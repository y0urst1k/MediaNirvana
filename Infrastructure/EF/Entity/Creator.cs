namespace Infrastructure.EF.Entity
{
    public class Creator : Base.Entity
    {
        public string Name { get; set; } = null!;
        public List<MediaItemCreator> MediaItemCreators { get; set; } = new();
    }
}