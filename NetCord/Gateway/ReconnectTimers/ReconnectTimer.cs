namespace NetCord.Gateway.ReconnectTimers;

public class ReconnectTimer : IReconnectTimer
{
    private int _delay;
    private CancellationTokenSource? _internalTokenSource;

    public async ValueTask NextAsync(CancellationToken token = default)
    {
        var delay = _delay;
        if (delay == 0)
        {
            token.ThrowIfCancellationRequested();
            _delay = 10_000;
        }
        else
        {
            if (delay < ((5 * 60) + 20) * 1000) // 5 minutes 20 seconds
                _delay = delay * 2;

            _internalTokenSource?.Dispose();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, (_internalTokenSource = new()).Token);
            await Task.Delay(delay, linkedTokenSource.Token).ConfigureAwait(false);
        }
    }

    public void Reset()
    {
        _delay = 0;
        _internalTokenSource?.Cancel();
    }

    public void Dispose()
    {
        _internalTokenSource?.Dispose();
    }
}
