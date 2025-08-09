using System.Net;

namespace FlightBoard.Api.Exceptions;

public class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpException(HttpStatusCode statusCode, string message) : base(message)
        => StatusCode = statusCode;
}