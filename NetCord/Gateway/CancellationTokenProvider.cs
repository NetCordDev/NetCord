namespace NetCord.Gateway;

internal sealed class CancellationTokenProvider : IDisposable
{
    private readonly CancellationTokenSource _source;

    public CancellationToken Token { get; }

    public CancellationTokenProvider()
    {
        var source = _source = new();
        Token = source.Token;
    }

    public void Cancel()
    {
        var source = _source;
        source.Cancel();
        source.Dispose();
    }

    public void Dispose()
    {
        _source.Dispose();
    }
}
