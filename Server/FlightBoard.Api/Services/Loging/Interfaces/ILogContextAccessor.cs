namespace FlightBoard.Api.Services.Logging.Interfaces;

public interface ILogContextAccessor
{
    string CorrelationId { get; set; }
    string Method { get; set; }
    string Path { get; set; }
    string Ip { get; set; }
    string UserAgent { get; set; }
    long ElapsedMs { get; set; } 
}
