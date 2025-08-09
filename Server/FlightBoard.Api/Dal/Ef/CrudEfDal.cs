using System.Linq.Expressions;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBoard.Api.Dal.Ef;

public class CrudEfDal<T>(AppDbContext db) : ICrudDal<T> where T : class
{
    private readonly AppDbContext _db = db;

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Set<T>().FindAsync([id], ct).AsTask();

    public Task<List<T>> GetAllAsync(CancellationToken ct = default)
        => _db.Set<T>().AsNoTracking().ToListAsync(ct);

    public async Task<List<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<T> q = _db.Set<T>().AsNoTracking();
        if (predicate is not null) q = q.Where(predicate);
        if (skip is not null)      q = q.Skip(skip.Value);
        if (take is not null)      q = q.Take(take.Value);
        return await q.ToListAsync(ct);
    }

    public Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => _db.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, ct);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => _db.Set<T>().AnyAsync(predicate, ct);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null
            ? _db.Set<T>().CountAsync(ct)
            : _db.Set<T>().CountAsync(predicate, ct);

    public Task AddAsync(T entity, CancellationToken ct = default)
        => _db.Set<T>().AddAsync(entity, ct).AsTask();

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        _db.Set<T>().AddRange(entities);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        _db.Set<T>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return;
        _db.Set<T>().Remove(entity);
    }
}
