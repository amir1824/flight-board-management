using FlightBoard.Api.Dal.DbModels.BaseModels; 

namespace FlightBoard.Api.Dal.DbModels;

public class Flight : CRUDModel
{
    public string FlightNumber { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateTime DepartureTime { get; set; } 
    public string Gate { get; set; } = null!;
}
