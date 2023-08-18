namespace NetCord.Gateway.Compression;

public class UncompressedGatewayCompression : IGatewayCompression
{
    public string Name => "uncompressed";

    public ReadOnlySpan<byte> Decompress(ReadOnlyMemory<byte> payload) => payload.Span;

    public void Initialize()
    {
    }

    public void Dispose()
    {
    }
}
