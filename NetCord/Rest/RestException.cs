using System.Net;

namespace NetCord.Rest;

public class RestException : Exception
{
    public RestException(HttpStatusCode statusCode, string reasonPhrase) : base($"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}).")
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
    }

    public RestException(HttpStatusCode statusCode, string reasonPhrase, RestError error) : base($"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}). {error.Message}.")
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase!;
        Error = error;
    }

    public HttpStatusCode StatusCode { get; }

    public string ReasonPhrase { get; }

    public RestError? Error { get; }
}
