using Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class CreatorConfig : IEntityTypeConfiguration<Creator>
    {
        public void Configure(EntityTypeBuilder<Creator> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(300);
            b.HasIndex(x => x.Name);
        }
    }
}