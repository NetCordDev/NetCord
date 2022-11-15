using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

internal class JsonContent<T> : HttpContent
{
    private readonly T _objToSerialize;
    private readonly JsonTypeInfo<T> _jsonTypeInfo;

    public JsonContent(T objToSerialize, JsonTypeInfo<T> jsonTypeInfo)
    {
        _objToSerialize = objToSerialize;
        _jsonTypeInfo = jsonTypeInfo;
        Headers.ContentType = new("application/json");
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => JsonSerializer.SerializeAsync(stream, _objToSerialize, _jsonTypeInfo);

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }
}
