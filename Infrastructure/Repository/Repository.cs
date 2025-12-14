using System.Linq.Expressions;
using Infrastructure.EF.Context;
using Infrastructure.EF.Entity.Base;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : Entity, new()
    {
        private readonly MediaNirvanaDbContext _db;
        private readonly DbSet<T> _set;

        public Repository(MediaNirvanaDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _set = _db.Set<T>();
        }

        protected virtual IQueryable<T> BuildQuery(bool asNoTracking = true, IEnumerable<Expression<Func<T, object>>>? includes = null)
        {
            IQueryable<T> q = _set;
            if (asNoTracking) 
                q = q.AsNoTracking();
            if (includes != null)
            {
                foreach (var inc in includes)
                    q = q.Include(inc);
            }
            return q;
        }

        public IQueryable<T> Query(bool asNoTracking = true, IEnumerable<Expression<Func<T, object>>>? includes = null)
            => BuildQuery(asNoTracking, includes);

        public async Task<T?> GetEntityAsync(Guid id, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
        {
            if (id == Guid.Empty) 
                return null;
            var q = BuildQuery(true, includes);
            return await q.FirstOrDefaultAsync(e => e.Id == id, ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync(CancellationToken ct = default, Func<IQueryable<T>, IQueryable<T>>? modifier = null)
        {
            IQueryable<T> q = BuildQuery(true, null);
            if (modifier != null) 
                q = modifier(q);
            return await q.ToListAsync(ct).ConfigureAwait(false);
        }

        public virtual async Task<T> AddEntityAsync(T entity, CancellationToken ct = default)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));
            await _set.AddAsync(entity, ct).ConfigureAwait(false);
            await _db.SaveChangesAsync(ct).ConfigureAwait(false);
            return entity;
        }

        public virtual async Task UpdateEntityAsync(T entity, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _set.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public virtual async Task DeleteEntityAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty) 
                return;
            var entity = await _set.FindAsync(new object[] { id }, ct).ConfigureAwait(false);
            if (entity == null) 
                return;
            _set.Remove(entity);
            await _db.SaveChangesAsync(ct).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await _set.AnyAsync(e => e.Id == id, ct);
        }

        public MediaNirvanaDbContext DbContext => _db;
    }
}