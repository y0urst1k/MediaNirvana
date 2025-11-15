using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class UserLabelConfig : IEntityTypeConfiguration<UserLabel>
    {
        public void Configure(EntityTypeBuilder<UserLabel> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(300);
            b.HasMany(x => x.Links).WithOne(l => l.UserLabel).HasForeignKey(l => l.UserLabelId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new 
            { 
                x.UserId, 
                x.Type 
            });
        }
    }
}