
namespace FlightBoard.Api.Middleware;

public static class RequestLogContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogContext(this IApplicationBuilder app)
        => app.UseMiddleware<RequestLogContextMiddleware>();
}
