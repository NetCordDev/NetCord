namespace NetCord.Gateway.Compression;

public interface IGatewayCompression : IDisposable
{
    public string Name { get; }

    public ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> payload);

    public void Initialize();
}
