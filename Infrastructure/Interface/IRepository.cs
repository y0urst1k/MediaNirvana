using System.Linq.Expressions;
using Infrastructure.EF.Context;

namespace Infrastructure.Interface
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        Task<T?> GetEntityAsync(Guid id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllEntitiesAsync(CancellationToken ct = default, Func<IQueryable<T>, IQueryable<T>>? modifier = null);
        Task<T> AddEntityAsync(T entity, CancellationToken ct = default);
        Task UpdateEntityAsync(T entity, CancellationToken ct = default);
        Task DeleteEntityAsync(Guid id, CancellationToken ct = default);
        IQueryable<T> Query(bool asNoTracking = true, IEnumerable<Expression<Func<T, object>>>? includes = null);

        MediaNirvanaDbContext DbContext { get; }
    }
}