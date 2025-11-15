using Infrastructure.EF.Entity.Base;
using Infrastructure.Interface;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service
{
    public class Service<T> : IService<T> where T : Entity, new()
    {
        private readonly IRepository<T> _repo;
        private readonly ILogger<Service<T>> _logger;
        public Service(IRepository<T> repo, ILogger<Service<T>> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> CreateItemAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item == null) 
                throw new ArgumentNullException(nameof(item));

            try
            {
                var created = await _repo.AddEntityAsync(item, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Сущность создана с Id = {Id}", created.Id);
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания");
                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("Id is empty", nameof(id));
            var existing = await _repo.GetEntityAsync(id, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                _logger.LogWarning("Сущность для удаления не найдена по Id = {Id}", id);
                return false;
            }

            try
            {
                await _repo.DeleteEntityAsync(id, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Сущность с Id = {Id} удалена", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления сущности с Id = {Id}", id);
                throw;
            }
        }

        public virtual async Task<T?> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("Id не найден", nameof(id));
            return await _repo.GetEntityAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(CancellationToken cancellationToken = default)
        {
            return await _repo.GetAllEntitiesAsync(null, cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> UpdateItemAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item == null) 
                throw new ArgumentNullException(nameof(item));
            if (item.Id == Guid.Empty) 
                throw new ArgumentException("Id не найден", nameof(item.Id));

            var existing = await _repo.GetEntityAsync(item.Id, cancellationToken).ConfigureAwait(false);
            if (existing == null)
                throw new KeyNotFoundException($"Сущность с Id = {item.Id} не найдена");

            try
            {
                await _repo.UpdateEntityAsync(existing, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Сущность с {Id} изменена", existing.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка изменения сущности с Id = {Id}", item.Id);
                throw;
            }
        }
    }
}