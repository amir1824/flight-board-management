using System.ComponentModel.DataAnnotations;

namespace FlightBoard.Api.Features.Flights.Requests;

public class CreateFlightRequest
{
    [Required, StringLength(10)]
    public string FlightNumber { get; set; } = null!;

    [Required, StringLength(80)]
    public string Destination { get; set; } = null!;

    [Required]
    public DateTime DepartureTime { get; set; } 

    [Required, StringLength(10)]
    public string Gate { get; set; } = null!;
}
