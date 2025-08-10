using FlightBoard.Api.Services.Logging.Interfaces;

namespace FlightBoard.Api.Services.Logging.Implementations;

public class LogContextAccessor : ILogContextAccessor
{
    public string CorrelationId { get; set; } = "";
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";
    public string Ip { get; set; } = "unknown";
    public string UserAgent { get; set; } = "";
    public long ElapsedMs { get; set; }
}
