using FlightBoard.Api.Dal.DbModels;

namespace FlightBoard.Api.Dal.Interfaces;

public interface IFlightDal : ICrudDal<Flight>
{
    Task<bool> ExistsByNumberAsync(string flightNumber, CancellationToken ct = default);
    Task<List<Flight>> SearchAsync(string? destination, CancellationToken ct = default);
}
