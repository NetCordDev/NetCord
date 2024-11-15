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

    public unsafe ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> payload)
    {
        var writer = _writer;
        var zstdStream = _zstdStream!;

        writer.Clear();

        int inLength = payload.Length;
        fixed (byte* payloadPtr = payload)
        {
            Zstandard.Buffer input = new()
            {
                Ptr = payloadPtr,
                Size = (nuint)inLength,
            };

            while (true)
            {
                var outBuffer = writer.GetSpan();
                int outLength = outBuffer.Length;
                Zstandard.Buffer output;
                nuint result;

                fixed (byte* outPtr = outBuffer)
                {
                    output = new()
                    {
                        Ptr = outPtr,
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

        return writer.WrittenSpan;
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
