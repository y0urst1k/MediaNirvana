using Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class MediaItemGenreConfig : IEntityTypeConfiguration<MediaItemGenre>
    {
        public void Configure(EntityTypeBuilder<MediaItemGenre> b)
        {
            b.HasKey(x => new 
            { 
                x.MediaItemId, 
                x.GenreId 
            });
            b.HasOne(x => x.MediaItem).WithMany(m => m.MediaItemGenres).HasForeignKey(x => x.MediaItemId);
            b.HasOne(x => x.Genre).WithMany(g => g.MediaItemGenres).HasForeignKey(x => x.GenreId);
        }
    }
}