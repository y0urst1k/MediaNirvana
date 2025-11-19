using Infrastructure.EF.Entity.ConnectingEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class UserInteractionTagConfig : IEntityTypeConfiguration<UserInteractionTag>
    {
        public void Configure(EntityTypeBuilder<UserInteractionTag> b)
        {
            b.HasKey(x => new 
            { 
                x.UserInteractionId, 
                x.TagId 
            });
            b.HasOne(x => x.UserInteraction).WithMany(u => u.Tags).HasForeignKey(x => x.UserInteractionId);
            b.HasOne(x => x.Tag).WithMany(t => t.UserInteractionTags).HasForeignKey(x => x.TagId);
        }
    }
}