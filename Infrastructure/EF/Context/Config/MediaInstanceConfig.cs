using Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class MediaInstanceConfig : IEntityTypeConfiguration<MediaInstance>
    {
        public void Configure(EntityTypeBuilder<MediaInstance> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Location).HasMaxLength(500);
            b.Property(x => x.SerialNumber).HasMaxLength(200);
            b.HasMany(x => x.Interactions).WithOne(i => i.MediaInstance).HasForeignKey(i => i.MediaInstanceId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.MediaItemId);
        }
    }
}