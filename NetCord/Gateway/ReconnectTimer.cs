namespace NetCord.Gateway;

internal class ReconnectTimer
{
    public int Delay { get; private set; }
    private CancellationTokenSource? _linkedTokenSource;
    private CancellationTokenSource? _internalTokenSource;

    public Task NextAsync(CancellationToken token = default)
    {
        if (Delay == 0)
        {
            Delay = 30_000;
            return Task.CompletedTask;
        }
        else
            if (Delay < 960_000)
            {
                var delay = Delay;
                Delay *= 2;
                _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, (_internalTokenSource = new()).Token);
                return Task.Delay(delay, _linkedTokenSource.Token);
            }
            else
                return Task.Delay(Delay, token);
    }

    public void Reset()
    {
        Delay = 0;
        _internalTokenSource?.Cancel();
    }
}