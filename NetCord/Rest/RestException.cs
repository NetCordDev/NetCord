using System.Diagnostics;
using System.Net;

namespace NetCord.Rest;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class RestException : Exception
{
    public RestException(HttpResponseMessage httpResponseMessage) : base($"Response status code does not indicate success: {(int)httpResponseMessage.StatusCode} ({httpResponseMessage.ReasonPhrase}).")
    {
        StatusCode = httpResponseMessage.StatusCode;
        ReasonPhrase = httpResponseMessage.ReasonPhrase!;
        ResponseContent = httpResponseMessage.Content;
    }

    public HttpStatusCode StatusCode { get; }

    public string ReasonPhrase { get; }

    public HttpContent ResponseContent { get; }

    public Task<string> GetDiscordErrorMessageAsync() => ResponseContent.ReadAsStringAsync();

    private string DebuggerDisplay()
    {
        using MemoryStream memoryStream = new();
        ResponseContent.CopyTo(memoryStream, null, default);
        memoryStream.Position = 0;
        using StreamReader streamReader = new(memoryStream);
        return streamReader.ReadToEnd();
    }
}
