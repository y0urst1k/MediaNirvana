using Infrastructure.EF.Entity.ConnectingEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class MediaItemCreatorConfig : IEntityTypeConfiguration<MediaItemCreator>
    {
        public void Configure(EntityTypeBuilder<MediaItemCreator> b)
        {
            b.HasKey(x => new 
            {
                x.MediaItemId,
                x.CreatorId
            });
            b.Property(x => x.Role).HasMaxLength(100);
            b.HasOne(x => x.MediaItem).WithMany(m => m.MediaItemCreators).HasForeignKey(x => x.MediaItemId);
            b.HasOne(x => x.Creator).WithMany(c => c.MediaItemCreators).HasForeignKey(x => x.CreatorId);
        }
    }
}