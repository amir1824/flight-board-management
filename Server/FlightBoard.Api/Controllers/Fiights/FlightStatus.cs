namespace FlightBoard.Api.Features.Flights;

public static class FlightStatus
{
    public static string From(DateTime departureUtc, DateTime nowUtc)
    {
        var until = departureUtc - nowUtc;
        if (until > TimeSpan.FromMinutes(30)) return "Scheduled";
        if (until >= TimeSpan.Zero)          return "Boarding";
        var after = nowUtc - departureUtc;
        if (after <= TimeSpan.FromMinutes(60)) return "Departed";
        return "Landed";
    }
}
