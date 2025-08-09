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

    protected override Task AfterCreateAsync(Flight entity, CancellationToken ct)
        => _hub.Clients.All.SendAsync("FlightAdded", MapToResponse(entity), ct);

    protected override Task AfterUpdateAsync(Flight entity, CancellationToken ct)
        => _hub.Clients.All.SendAsync("FlightUpdated", MapToResponse(entity), ct);

    protected override Task AfterDeleteAsync(int id, CancellationToken ct)
        => _hub.Clients.All.SendAsync("FlightDeleted", id, ct);
}
