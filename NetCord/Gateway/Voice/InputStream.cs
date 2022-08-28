using NetCord.Gateway.Voice.Streams;

namespace NetCord.Gateway.Voice;

internal class InputStream
{
    public InputStream(ToMemoryStream stream, Stream writeStream)
    {
        Stream = stream;
        WriteStream = writeStream;
    }

    public ToMemoryStream Stream { get; }

    public Stream WriteStream { get; }
}
