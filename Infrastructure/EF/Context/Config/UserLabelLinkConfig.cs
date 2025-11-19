using Infrastructure.EF.Entity.ConnectingEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class UserLabelLinkConfig : IEntityTypeConfiguration<UserLabelLink>
    {
        public void Configure(EntityTypeBuilder<UserLabelLink> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.TargetId).IsRequired();
            b.HasIndex(x => new 
            { 
                x.UserLabelId, 
                x.TargetType 
            });
        }
    }
}