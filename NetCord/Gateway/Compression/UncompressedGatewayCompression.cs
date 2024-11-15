namespace NetCord.Gateway.Compression;

public sealed class UncompressedGatewayCompression : IGatewayCompression
{
    public string Name => "uncompressed";

    public ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> payload) => payload;

    public void Initialize()
    {
    }

    public void Dispose()
    {
    }
}
