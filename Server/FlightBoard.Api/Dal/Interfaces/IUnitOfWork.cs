namespace FlightBoard.Api.Dal.Interfaces;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
