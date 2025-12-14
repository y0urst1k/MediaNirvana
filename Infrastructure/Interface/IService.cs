using System.Linq.Expressions;

namespace Infrastructure.Interface
{
    public interface IService<T> where T : class, IEntity, new()
    {
        Task<T?> GetItemAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetItemsAsync(CancellationToken cancellationToken = default, Expression<Func<T, bool>>? filter = null);
        Task<T> CreateItemAsync(T item, CancellationToken cancellationToken = default);
        Task<T> UpdateItemAsync(T item, CancellationToken cancellationToken = default);
        Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
    }
}