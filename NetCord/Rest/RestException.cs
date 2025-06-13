using System.Net;

namespace NetCord.Rest;

public class RestException : Exception
{
    public RestException(HttpStatusCode statusCode, string? reasonPhrase) : base(GetMessage(statusCode, reasonPhrase))
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
    }

    public RestException(HttpStatusCode statusCode, string? reasonPhrase, RestError error) : base(GetMessage(statusCode, reasonPhrase, error.Message))
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        Error = error;
    }

    private static string GetMessage(HttpStatusCode statusCode, string? reasonPhrase)
    {
        return string.IsNullOrEmpty(reasonPhrase)
            ? $"Response status code does not indicate success: {(int)statusCode}."
            : $"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}).";
    }

    private static string GetMessage(HttpStatusCode statusCode, string? reasonPhrase, string errorMessage)
    {
        var errorMessageSpan = errorMessage.AsSpanFast();
        if (string.IsNullOrEmpty(reasonPhrase))
        {
            return errorMessageSpan is [.., '.']
                ? $"Response status code does not indicate success: {(int)statusCode}. {errorMessage}"
                : $"Response status code does not indicate success: {(int)statusCode}. {errorMessage}.";
        }
        else
        {
            return errorMessageSpan is [.., '.']
                ? $"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}). {errorMessage}"
                : $"Response status code does not indicate success: {(int)statusCode} ({reasonPhrase}). {errorMessage}.";
        }
    }

    public HttpStatusCode StatusCode { get; }

    public string? ReasonPhrase { get; }

    public RestError? Error { get; }

    public override string ToString()
    {
        const string ClassName = "NetCord.Rest.RestException";

        var message = Message;
        var stackTrace = StackTrace;
        string? jsonError;

        int length = ClassName.Length + 2 + message.Length;

        if (Error is { } error)
        {
            jsonError = error.ToString();
            length += Environment.NewLine.Length + jsonError.Length;
        }
        else
            jsonError = null;

        if (stackTrace is not null)
            length += Environment.NewLine.Length + stackTrace.Length;

        return string.Create(length, (Message: message, StackTrace: stackTrace, JsonError: jsonError), static (span, state) =>
        {
            Write(ClassName, ref span);

            Write(": ", ref span);
            Write(state.Message, ref span);

            if (state.JsonError is { } jsonError)
            {
                Write(Environment.NewLine, ref span);
                Write(jsonError, ref span);
            }

            if (state.StackTrace is { } stackTrace)
            {
                Write(Environment.NewLine, ref span);
                Write(stackTrace, ref span);
            }
        });

        static void Write(string source, ref Span<char> dest)
        {
            source.CopyTo(dest);
            dest = dest[source.Length..];
        }
    }
}
