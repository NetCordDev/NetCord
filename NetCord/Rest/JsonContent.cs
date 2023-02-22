using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

public class JsonContent<T> : HttpContent
{
    private readonly T _objToSerialize;
    private readonly JsonTypeInfo<T> _jsonTypeInfo;

    public JsonContent(T objToSerialize, JsonTypeInfo<T> jsonTypeInfo)
    {
        Headers.ContentType = (MediaTypeHeaderValue)((ICloneable)JsonContent._contentType).Clone();
        _objToSerialize = objToSerialize;
        _jsonTypeInfo = jsonTypeInfo;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => JsonSerializer.SerializeAsync(stream, _objToSerialize, _jsonTypeInfo);

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }
}

file static class JsonContent
{
    internal static readonly MediaTypeHeaderValue _contentType = new("application/json");
}
