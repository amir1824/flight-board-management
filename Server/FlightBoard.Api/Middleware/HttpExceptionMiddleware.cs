using System.Net;
using System.Text.Json;
using FlightBoard.Api.Exceptions;
using FlightBoard.Api.Services.Logging.Interfaces;
using Serilog.Context; 

namespace FlightBoard.Api.Middleware;

public class HttpExceptionMiddleware
{
  private readonly RequestDelegate _next;

  public HttpExceptionMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context, IAppLogger appLogger)
  {
    try
    {
      await _next(context);

    }
    catch (Exception ex)
    {
      var status = ex is HttpException httpEx ? (int)httpEx.StatusCode : (int)HttpStatusCode.InternalServerError;

      context.Response.Clear();
      context.Response.StatusCode = status;
      context.Response.ContentType = "application/json; charset=utf-8";

      using (LogContext.PushProperty("Operation", "HTTP"))
      using (LogContext.PushProperty("Entity", $"{context.Request.Method} {context.Request.Path}"))
      using (LogContext.PushProperty("StatusCode", status))
      {
        appLogger.Error(ex, "Unhandled exception", new {
          Path = context.Request.Path.Value,
          TraceId = context.TraceIdentifier
        });
      }

      var payload = JsonSerializer.Serialize(new {
        message = ex.Message,
        statusCode = status,
        path = context.Request.Path.Value,
        traceId = context.TraceIdentifier
      });

      await context.Response.WriteAsync(payload);
    }
  }
}
