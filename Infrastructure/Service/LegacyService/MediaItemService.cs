using System.Linq.Expressions;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service.LegacyService
{
    public class MediaItemService : Service<MediaItem>
    {
        public MediaItemService(IRepository<MediaItem> repo, ILogger<MediaItemService> logger)
            : base(repo, logger)
        {
        }

        // Переопределяем получение списка
        public override async Task<IEnumerable<MediaItem>> GetItemsAsync(
            CancellationToken cancellationToken = default,
            Expression<Func<MediaItem, bool>>? filter = null)
        {
            IQueryable<MediaItem> query = _repo.Query(asNoTracking: true);

            // Жадная загрузка (Eager Loading)
            query = query
                .Include(x => x.MediaItemGenres)
                .Include(x => x.MediaItemCreators)
                .Include(x => x.Instances); // Загружаем экземпляры, чтобы знать, есть ли файл

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(cancellationToken);
        }

        // Переопределяем получение одного элемента
        public override async Task<MediaItem?> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repo.Query(asNoTracking: true)
            .Include(x => x.MediaItemGenres)
            .Include(x => x.MediaItemCreators)
            .Include(x => x.Instances)
                .ThenInclude(i => i.Interactions) // Для деталей грузим и оценки
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
