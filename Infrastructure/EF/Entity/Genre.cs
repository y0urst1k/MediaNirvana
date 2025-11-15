namespace Infrastructure.EF.Entity
{
    public class Genre : Base.Entity
    {
        public string Name { get; set; } = null!;
        public List<MediaItemGenre> MediaItemGenres { get; set; } = new();
    }
}