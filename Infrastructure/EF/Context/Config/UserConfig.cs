using Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Username).IsRequired().HasMaxLength(200);
            b.HasIndex(x => x.Username).IsUnique();
            b.HasMany(x => x.Interactions).WithOne(i => i.User).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Labels).WithOne(l => l.User).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}