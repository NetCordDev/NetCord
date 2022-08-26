using System.Diagnostics;
using System.Net;

namespace NetCord.Rest;

[DebuggerDisplay("{GetDiscordErrorMessageAsync().Result}")]
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
}
