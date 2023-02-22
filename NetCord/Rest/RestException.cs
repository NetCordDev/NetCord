using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace NetCord.Rest;

[Serializable]
[DebuggerDisplay("{GetDiscordErrorMessageAsync().Result}")]
public class RestException : Exception
{
    public RestException(HttpResponseMessage httpResponseMessage) : base($"Response status code does not indicate success: {(int)httpResponseMessage.StatusCode} ({httpResponseMessage.ReasonPhrase}).")
    {
        StatusCode = httpResponseMessage.StatusCode;
        ReasonPhrase = httpResponseMessage.ReasonPhrase!;
        ResponseContent = httpResponseMessage.Content;
    }

    protected RestException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        StatusCode = (HttpStatusCode)serializationInfo.GetInt32(nameof(StatusCode));
        ReasonPhrase = serializationInfo.GetString(nameof(ReasonPhrase))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(StatusCode), (int)StatusCode);
        info.AddValue(nameof(ReasonPhrase), ReasonPhrase);
    }

    public HttpStatusCode StatusCode { get; }

    public string ReasonPhrase { get; }

    [AllowNull]
    [field: NonSerialized]
    public HttpContent ResponseContent { get; }

    public Task<string> GetDiscordErrorMessageAsync() => ResponseContent.ReadAsStringAsync();
}
