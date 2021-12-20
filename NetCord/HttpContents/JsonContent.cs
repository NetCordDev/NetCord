using System.Net;
using System.Text;
using System.Text.Json;

namespace NetCord;

internal class JsonContent : HttpContent
{
    private readonly Stream _stream;
    private readonly long _start;
    private bool _used = false;

    public JsonContent(string json) : this()
    {
        _stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        _start = 0;
    }

    public JsonContent(JsonDocument json) : this()
    {
        _stream = new MemoryStream();
        JsonSerializer.Serialize(_stream, json);
        _stream.Position = 0;
        _start = 0;
    }

    public JsonContent(Stream jsonStream) : this()
    {
        _stream = jsonStream;
        if (_stream.CanSeek)
            _start = _stream.Position;
    }

    public JsonContent(object objToSerialize) : this()
    {
        _stream = new MemoryStream();
        JsonSerializer.Serialize(_stream, objToSerialize);
        _stream.Position = 0;
        _start = 0;
    }

    private JsonContent()
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