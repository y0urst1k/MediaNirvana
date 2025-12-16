using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service.LegacyService
{
    public class UserLabelService : Service<UserLabel>
    {
        public UserLabelService(IRepository<UserLabel> repo, ILogger<UserLabelService> logger)
            : base(repo, logger)
        {
        }

        public override async Task<IEnumerable<UserLabel>> GetItemsAsync(CancellationToken cancellationToken = default, System.Linq.Expressions.Expression<Func<UserLabel, bool>>? filter = null)
        {
            IQueryable<UserLabel> query = _repo.Query(asNoTracking: true)
                .Include(ul => ul.Links); // Загружаем список ID элементов в этом списке

            if (filter != null) query = query.Where(filter);

            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<UserLabel?> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repo.Query(asNoTracking: true)
                .Include(ul => ul.Links)
                .FirstOrDefaultAsync(ul => ul.Id == id, cancellationToken);
        }
    }
}
