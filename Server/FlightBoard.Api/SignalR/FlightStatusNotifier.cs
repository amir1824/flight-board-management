using FlightBoard.Api.Data;
using FlightBoard.Api.Features.Flights;
using FlightBoard.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class FlightStatusNotifier : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IHubContext<FlightsHub> _hub;

    public FlightStatusNotifier(IServiceProvider sp, IHubContext<FlightsHub> hub)
    {
        _sp = sp; _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var last = new Dictionary<int, string>();

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.UtcNow;
            var flights = await db.Flights.AsNoTracking().ToListAsync(stoppingToken);

            foreach (var f in flights)
            {
                var status = FlightStatus.From(f.DepartureTime, now);
                if (!last.TryGetValue(f.Id, out var prev) || prev != status)
                {
                    last[f.Id] = status;
                    await _hub.Clients.All.SendAsync("FlightStatusChanged",
                        new { id = f.Id, status }, stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); 
        }
    }
}
