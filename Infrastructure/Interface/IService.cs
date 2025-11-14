namespace Infrastructure.Interface
{
    public interface IService<T> where T : class, IEntity, new()
    {
        IEnumerable<T> ServiceItems { get; }

        Task<T> MakeItem(T entity);

        Task UpdateItem(T entity);

        Task DeleteItem(int id);

        Task<IEnumerable<T>> GetItems();

        Task<T> GetItem(int id);
    }
}