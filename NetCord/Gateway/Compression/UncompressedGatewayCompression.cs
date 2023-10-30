namespace NetCord.Gateway.Compression;

public class UncompressedGatewayCompression : IGatewayCompression
{
    public string Name => "uncompressed";

    public ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> payload) => payload;

    public void Initialize()
    {
    }

    public void Dispose()
    {
    }
}
