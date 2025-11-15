using Infrastructure.EF.Entity.IndependentEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class MediaItemConfig : IEntityTypeConfiguration<MediaItem>
    {
        public void Configure(EntityTypeBuilder<MediaItem> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired();
            b.HasMany(x => x.Instances).WithOne(x => x.MediaItem).HasForeignKey(x => x.MediaItemId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.MediaItemGenres).WithOne(mg => mg.MediaItem).HasForeignKey(mg => mg.MediaItemId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.MediaItemCreators).WithOne(mc => mc.MediaItem).HasForeignKey(mc => mc.MediaItemId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.RelationshipsSource).WithOne(r => r.SourceMedia).HasForeignKey(r => r.SourceMediaId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.RelationshipsTarget).WithOne(r => r.TargetMedia).HasForeignKey(r => r.TargetMediaId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => x.Title);
            b.HasIndex(x => x.Year);
        }
    }
}