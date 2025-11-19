using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class UserInteractionConfig : IEntityTypeConfiguration<UserInteraction>
    {
        public void Configure(EntityTypeBuilder<UserInteraction> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.PersonalRating).HasColumnType("decimal(3,1)");
            b.HasMany(x => x.Tags).WithOne(t => t.UserInteraction).HasForeignKey(t => t.UserInteractionId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new 
            { 
                x.UserId, 
                x.MediaInstanceId 
            }).IsUnique(false);
        }
    }
}