using Infrastructure.EF.Context.Config;
using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF.Context
{
    public class MediaNirvanaDbContext : DbContext
    {
        public MediaNirvanaDbContext(DbContextOptions<MediaNirvanaDbContext> opts) : base(opts) { }

        public DbSet<MediaItem> MediaItems => Set<MediaItem>();
        public DbSet<MediaInstance> MediaInstances => Set<MediaInstance>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserInteraction> UserInteractions => Set<UserInteraction>();
        public DbSet<UserLabel> UserLabels => Set<UserLabel>();
        public DbSet<UserLabelLink> UserLabelLinks => Set<UserLabelLink>();
        public DbSet<Relationship> Relationships => Set<Relationship>();

        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<MediaItemGenre> MediaItemGenres => Set<MediaItemGenre>();
        public DbSet<Creator> Creators => Set<Creator>();
        public DbSet<MediaItemCreator> MediaItemCreators => Set <MediaItemCreator>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<UserInteractionTag> UserInteractionTags => Set<UserInteractionTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MediaItemConfig());
            modelBuilder.ApplyConfiguration(new MediaInstanceConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new UserInteractionConfig());
            modelBuilder.ApplyConfiguration(new UserLabelConfig());
            modelBuilder.ApplyConfiguration(new UserLabelLinkConfig());
            modelBuilder.ApplyConfiguration(new RelationshipConfig());

            modelBuilder.ApplyConfiguration(new GenreConfig());
            modelBuilder.ApplyConfiguration(new MediaItemGenreConfig());
            modelBuilder.ApplyConfiguration(new CreatorConfig());
            modelBuilder.ApplyConfiguration(new MediaItemCreatorConfig());
            modelBuilder.ApplyConfiguration(new TagConfig());
            modelBuilder.ApplyConfiguration(new UserInteractionTagConfig());
        }
    }
}
