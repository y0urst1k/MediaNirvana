namespace Infrastructure.Interface
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        IQueryable<T> Items { get; }

        #region Синхронные операции
        T GetEntity(int id);

        T AddEntity(T item);

        void UpdateEntity(T item);

        void RemoveEntity(int id);
        #endregion

        #region Ассинхронные операции
        Task<T> GetEntityAsync(int id, CancellationToken canceltoken = default);

        Task<T> AddEntityAsync(T item, CancellationToken canceltoken = default);

        Task UpdateEntityAsync(T item, CancellationToken canceltoken = default);

        Task RemoveEntityAsync(int id, CancellationToken canceltoken = default);

        Task<IEnumerable<T>> GetAllEntitiesAsync(CancellationToken canceltoken = default);
        #endregion
    }
}