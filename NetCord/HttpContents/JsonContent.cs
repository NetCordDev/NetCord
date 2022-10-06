using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NetCord;

internal class JsonContent : HttpContent
{
    protected readonly Stream _stream;
    protected readonly long _start;
    private bool _used = false;

    public JsonContent(string json) : this()
    {
        _stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
    }

    public JsonContent(Stream jsonStream) : this()
    {
        _stream = jsonStream;
        if (_stream.CanSeek)
            _start = _stream.Position;
    }

    protected JsonContent()
    {
        Headers.ContentType = new("application/json");
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        if (_used)
        {
            if (_stream.CanSeek)
                _stream.Position = _start;
            else
                throw new InvalidOperationException("The stream was already consumed. It cannot be read again.");
        }
        else
            _used = true;

        return _stream.CopyToAsync(stream);
    }

    protected override bool TryComputeLength(out long length)
    {
        if (_stream.CanSeek)
        {
            length = _stream.Length - _start;
            return true;
        }
        else
        {
            length = 0;
            return false;
        }
    }
}

internal class JsonContent<T> : JsonContent
{
    public JsonContent(T objToSerialize, JsonTypeInfo<T> jsonTypeInfo) : base(CreateStream(objToSerialize, jsonTypeInfo))
    {
    }

    private static Stream CreateStream(T objToSerialize, JsonTypeInfo<T> jsonTypeInfo)
    {
        MemoryStream stream = new();
        JsonSerializer.Serialize(stream, objToSerialize, jsonTypeInfo);
        stream.Position = 0;
        return stream;
    }
}
