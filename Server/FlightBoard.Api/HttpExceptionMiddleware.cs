using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using FlightBoard.Api.Exceptions;

namespace FlightBoard.Api.Middleware;

public class HttpExceptionMiddleware(RequestDelegate next, ILogger<HttpExceptionMiddleware> logger)
{
  private readonly RequestDelegate _next = next;
  private readonly ILogger<HttpExceptionMiddleware> _logger = logger;

  public async Task Invoke(HttpContext context)
  {
    var sw = Stopwatch.StartNew();

    var req = context.Request;
    var method = req.Method;
    var path = req.Path.Value ?? "/";
    var ua = req.Headers.UserAgent.ToString();
    var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";
    var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

    try
    {
      await _next(context);
      sw.Stop();

      _logger.LogInformation("[OK] {Method} {Path} | {Status} | {Ms}ms | {UserId} | {IP} | UA={UA}",
          method, path, context.Response.StatusCode, sw.ElapsedMilliseconds, userId, ip, ua);
    }
    catch (Exception ex)
    {
      sw.Stop();

      var status = ex is HttpException httpEx ? (int)httpEx.StatusCode : (int)HttpStatusCode.InternalServerError;
      context.Response.StatusCode = status;
      context.Response.ContentType = "application/json; charset=utf-8";

      _logger.LogError(ex, "[ERR] {Method} {Path} | {Status} | {Ms}ms | {UserId} | {IP}",
          method, path, status, sw.ElapsedMilliseconds, userId, ip);

      var payload = JsonSerializer.Serialize(new
      {
        message = ex.Message,
        statusCode = status,
        path,
        traceId = context.TraceIdentifier
      });

      await context.Response.WriteAsync(payload);
    }
  }
}