using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.EF.Context
{
    public class MediaNirvanaDbContextFactory : IDesignTimeDbContextFactory<MediaNirvanaDbContext>
    {
        public MediaNirvanaDbContext CreateDbContext(string[] args)
        {
            var optionbuilder = new DbContextOptionsBuilder<MediaNirvanaDbContext>();
            optionbuilder.UseSqlite("Data Source=MediaNirvana.db");
            return new MediaNirvanaDbContext(optionbuilder.Options);
        }
    }
}