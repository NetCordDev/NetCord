namespace NetCord.Gateway.ReconnectTimers;

public interface IReconnectTimer : IDisposable
{
    public ValueTask NextAsync(CancellationToken token = default);
    public void Reset();
}
