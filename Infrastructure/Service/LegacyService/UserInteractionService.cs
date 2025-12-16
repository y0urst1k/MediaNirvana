using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service.LegacyService
{
    public class UserInteractionService : Service<UserInteraction>
    {
        public UserInteractionService(IRepository<UserInteraction> repo, ILogger<UserInteractionService> logger)
            : base(repo, logger)
        {
        }

        public override async Task<IEnumerable<UserInteraction>> GetItemsAsync(CancellationToken cancellationToken = default, System.Linq.Expressions.Expression<Func<UserInteraction, bool>>? filter = null)
        {
            IQueryable<UserInteraction> query = _repo.Query(asNoTracking: true)
                .Include(ui => ui.MediaInstance) // Чтобы знать формат/источник
                .Include(ui => ui.Tags)
                    .ThenInclude(uit => uit.Tag); // Чтобы фильтровать по тегам

            if (filter != null) query = query.Where(filter);

            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<UserInteraction?> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repo.Query(asNoTracking: true)
                .Include(ui => ui.MediaInstance)
                .Include(ui => ui.Tags)
                    .ThenInclude(uit => uit.Tag)
                .FirstOrDefaultAsync(ui => ui.Id == id, cancellationToken);
        }
    }
}
