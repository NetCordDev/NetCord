using static NetCord.Gateway.Compression.Zstandard;

namespace NetCord.Gateway.Compression;

public sealed partial class ZstandardGatewayCompression : IGatewayCompression
{
    private const int DefaultBufferSize = 8192;

    private readonly RentedArrayBufferWriter<byte> _writer;
    private readonly DStreamHandle _zstdStream;

    public ZstandardGatewayCompression()
    {
        _writer = new(DefaultBufferSize);
        _zstdStream = CreateDStream();
    }

    public string Name => "zstd-stream";

    public unsafe ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> payload)
    {
        var writer = _writer;
        var zstdStream = _zstdStream!;

        var inBuffer = payload.Span;
        int inLength = inBuffer.Length;
        fixed (byte* ptrIn = inBuffer)
        {
            Zstandard.Buffer input = new()
            {
                Ptr = ptrIn,
                Size = (nuint)inLength,
            };

            while (true)
            {
                var outBuffer = writer.GetSpan();
                int outLength = outBuffer.Length;
                Zstandard.Buffer output;
                nuint result;

                fixed (byte* ptrOut = outBuffer)
                {
                    output = new()
                    {
                        Ptr = ptrOut,
                        Size = (nuint)outLength,
                    };

                    result = DecompressStream(zstdStream, ref output, ref input);
                }

                if (IsError(result))
                    throw new ZstandardException(result);

                writer.Advance((int)output.Pos);

                if ((int)output.Pos != outLength && (int)input.Pos == inLength)
                    break;
            }
        }

        var written = writer.WrittenMemory;
        writer.Clear();
        return written;
    }

    public void Initialize()
    {
        InitDStream(_zstdStream);
    }

    public void Dispose()
    {
        _writer.Dispose();
        _zstdStream.Dispose();
    }
}
