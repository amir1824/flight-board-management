using System.Linq.Expressions;

namespace FlightBoard.Api.Dal.Interfaces;

public interface ICrudDal<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);

    Task<List<T>> ListAsync(
        Expression<Func<T, bool>>? predicate = null,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int>  CountAsync (Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task RemoveAsync(T entity, CancellationToken ct = default);
    Task RemoveByIdAsync(int id, CancellationToken ct = default);
}
