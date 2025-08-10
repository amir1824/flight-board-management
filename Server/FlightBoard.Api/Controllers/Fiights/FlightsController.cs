using System.Linq.Expressions;
using FlightBoard.Api.Dal.DbModels;
using FlightBoard.Api.Dal.Interfaces;
using FlightBoard.Api.Features.Flights;
using FlightBoard.Api.Features.Flights.Responses;
using FlightBoard.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlightBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController(
    IFlightDal dal,
    IUnitOfWork uow,
    IHubContext<FlightsHub> hub,
    ILogger<CrudControllerBase<Flight>> logger)
    : CrudControllerBase<Flight>(dal, uow, logger)
{
    private readonly IHubContext<FlightsHub> _hub = hub;

    private static FlightResponse MapToResponse(Flight f) => new()
    {
        Id = f.Id,
        FlightNumber = f.FlightNumber,
        Destination = f.Destination,
        DepartureTime = f.DepartureTime,
        Gate = f.Gate,
        Status = FlightStatus.From(f.DepartureTime, DateTime.UtcNow)
    };

    [HttpGet]
    public async Task<IEnumerable<FlightResponse>> GetAllView(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 100,
    CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);

        var skip = (page - 1) * pageSize;
        var now = DateTime.UtcNow;

        var items = await _crud.ListAsync(_ => true, skip, pageSize, ct);
        return items.Select(f => new FlightResponse
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            Destination = f.Destination,
            DepartureTime = f.DepartureTime,
            Gate = f.Gate,
            Status = FlightStatus.From(f.DepartureTime, now) 
        });
    }

    [HttpGet("search")]
    public async Task<IEnumerable<FlightResponse>> Search(
    [FromQuery] string? status,
    [FromQuery] string? destination,
    CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        Expression<Func<Flight, bool>> dbFilter = f => true;

        if (!string.IsNullOrWhiteSpace(destination))
        {
            var dest = destination.Trim().ToLower();
            dbFilter = f => f.Destination.ToLower().Contains(dest);
        }

        var items = await _crud.ListAsync(dbFilter, skip: 0, take: 500, ct);

        if (!string.IsNullOrWhiteSpace(status))
        {
            items = items.Where(f => FlightStatus.From(f.DepartureTime, now).Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return items.Select(MapToResponse);
    }

    protected override Task AfterCreateAsync(Flight entity, CancellationToken ct)
        => _hub.Clients.All.SendAsync("FlightAdded", MapToResponse(entity), ct);

    protected override Task AfterUpdateAsync(Flight entity, CancellationToken ct)
        => _hub.Clients.All.SendAsync("FlightUpdated", MapToResponse(entity), ct);

    protected override Task AfterDeleteAsync(int id, CancellationToken ct)
        => _hub.Clients.All.SendAsync("FlightDeleted", id, ct);
}
