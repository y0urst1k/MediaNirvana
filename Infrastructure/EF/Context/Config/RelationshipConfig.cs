using Infrastructure.EF.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EF.Context.Config
{
    public class RelationshipConfig : IEntityTypeConfiguration<Relationship>
    {
        public void Configure(EntityTypeBuilder<Relationship> b)
        {
            b.HasKey(x => x.Id); b.HasOne(x => x.SourceMedia).WithMany(m => m.RelationshipsSource).HasForeignKey(x => x.SourceMediaId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.TargetMedia).WithMany(m => m.RelationshipsTarget).HasForeignKey(x => x.TargetMediaId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new 
            { 
                x.SourceMediaId, 
                x.TargetMediaId 
            });
        }
    }
}