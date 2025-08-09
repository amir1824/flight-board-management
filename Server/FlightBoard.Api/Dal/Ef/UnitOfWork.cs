using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Data;

namespace FlightBoard.Api.Dal.Ef;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
