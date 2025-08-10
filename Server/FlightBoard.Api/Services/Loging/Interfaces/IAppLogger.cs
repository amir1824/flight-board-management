namespace FlightBoard.Api.Services.Logging.Interfaces;

public interface IAppLogger
{
  void Audit(string action, object? details = null);
  void Info(string message, object? details = null);
  void Warn(string message, object? details = null);
  void Error(Exception ex, string message, object? details = null);
}
