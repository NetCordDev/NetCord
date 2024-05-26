namespace NetCord.Gateway.Compression;

public interface IGatewayCompression : IDisposable
{
    internal static IGatewayCompression CreateDefault()
    {
        return ZStandard.TryLoad() ? new ZStandardGatewayCompression() : new ZLibGatewayCompression();
    }

    public string Name { get; }

    public ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> payload);

    public void Initialize();
}
