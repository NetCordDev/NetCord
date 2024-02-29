using System.Net;

namespace NetCord.Rest;

public class RestException : Exception
{
    public RestException(HttpStatusCode statusCode, string reasonPhrase) : base(GetMessage(statusCode, reasonPhrase))
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
    }

    public RestException(HttpStatusCode statusCode, string reasonPhrase, RestError error) : base(GetMessage(statusCode, reasonPhrase, error.Message))
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase!;
        Error = error;
    }

    private static string GetMessage(HttpStatusCode statusCode, string reasonPhrase)
    {
        return $"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}).";
    }

    private static string GetMessage(HttpStatusCode statusCode, string reasonPhrase, string errorMessage)
    {
        return errorMessage.EndsWith('.')
                ? $"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}). {errorMessage}"
                : $"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}). {errorMessage}.";
    }

    public HttpStatusCode StatusCode { get; }

    public string ReasonPhrase { get; }

    public RestError? Error { get; }
}
