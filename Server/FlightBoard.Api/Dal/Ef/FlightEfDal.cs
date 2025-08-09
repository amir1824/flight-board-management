using FlightBoard.Api.Dal.DbModels;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBoard.Api.Dal.Ef;

public class FlightEfDal(AppDbContext db) : CrudEfDal<Flight>(db), IFlightDal
{
    public Task<bool> ExistsByNumberAsync(string flightNumber, CancellationToken ct = default)
        => db.Flights.AnyAsync(f => f.FlightNumber == flightNumber, ct);

    public Task<List<Flight>> SearchAsync(string? destination, CancellationToken ct = default)
    {
        var q = db.Flights.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(destination))
            q = q.Where(f => EF.Functions.Like(f.Destination, $"%{destination}%"));
        return q.ToListAsync(ct);
    }
}
