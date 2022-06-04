using NetCord.Gateway.Voice.Streams;

namespace NetCord.Gateway.Voice;

internal class InputStream
{
    public InputStream(ToArrayStream stream, Stream writeStream)
    {
        Stream = stream;
        WriteStream = writeStream;
    }

    public ToArrayStream Stream { get; }

    public Stream WriteStream { get; }
}
