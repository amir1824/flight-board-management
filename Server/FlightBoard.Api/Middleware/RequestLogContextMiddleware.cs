using Serilog.Context;
using System.Diagnostics;
using FlightBoard.Api.Services.Logging.Interfaces;

public class RequestLogContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLogContextMiddleware> _logger;

    public RequestLogContextMiddleware(RequestDelegate next, ILogger<RequestLogContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, ILogContextAccessor logCtx)
    {
        var sw = Stopwatch.StartNew();

        logCtx.Method = context.Request.Method;
        logCtx.Path   = context.Request.Path.Value ?? "/";
        logCtx.UserAgent = context.Request.Headers.UserAgent.ToString();
        logCtx.Ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";
        logCtx.CorrelationId = context.TraceIdentifier;

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            logCtx.ElapsedMs = sw.ElapsedMilliseconds;

            using (LogContext.PushProperty("Operation", "HTTP"))
            using (LogContext.PushProperty("Entity", $"{logCtx.Method} {logCtx.Path}"))
            using (LogContext.PushProperty("StatusCode", context.Response.StatusCode))
            {
                _logger.LogInformation("HTTP {Method} {Path} responded {ElapsedMs} ms",
                    logCtx.Method, logCtx.Path, logCtx.ElapsedMs);
            }
        }
    }
}
