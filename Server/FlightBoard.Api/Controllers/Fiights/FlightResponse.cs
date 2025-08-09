namespace FlightBoard.Api.Features.Flights.Responses;

public class FlightResponse
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public string Gate { get; set; } = null!;
    public string Status { get; set; } = null!;
}
