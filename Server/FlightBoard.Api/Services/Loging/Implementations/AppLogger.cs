using FlightBoard.Api.Services.Logging.Interfaces;

namespace FlightBoard.Api.Services.Logging.Implementations;

public class AppLogger(ILogger<AppLogger> logger, ILogContextAccessor ctx) : IAppLogger
{
  private readonly ILogger<AppLogger> _logger = logger;
  private readonly ILogContextAccessor _ctx = ctx;

  private object Envelope(object? details) => new
  {
    _ctx.CorrelationId,
    _ctx.Method,
    _ctx.Path,
    _ctx.Ip,
    UA = _ctx.UserAgent,
    ElapsedMs = _ctx.ElapsedMs,
    Details = details
  };

  public void Audit(string action, object? details = null)
      => _logger.LogInformation("AUDIT {Action} {@ctx}", action, Envelope(details));

  public void Info(string message, object? details = null)
      => _logger.LogInformation("{Message} {@ctx}", message, Envelope(details));

  public void Warn(string message, object? details = null)
      => _logger.LogWarning("{Message} {@ctx}", message, Envelope(details));

  public void Error(Exception ex, string message, object? details = null)
      => _logger.LogError(ex, "{Message} {@ctx}", message, Envelope(details));
}
