using Microsoft.AspNetCore.Builder;

namespace FlightBoard.Api.Middleware
{
    public static class HttpExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpExceptionMiddleware>();
        }
    }
}
