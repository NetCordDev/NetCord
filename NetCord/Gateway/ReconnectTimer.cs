namespace NetCord.Gateway;

internal class ReconnectTimer
{
    public int Delay { get; private set; }
    private CancellationTokenSource? _internalTokenSource;

    public Task NextAsync(CancellationToken token = default)
    {
        if (Delay == 0)
        {
            Delay = 10_000;
            return Task.CompletedTask;
        }
        else if (Delay < 320_000) // 5 minutes 20 seconds
        {
            var delay = Delay;
            Delay *= 2;
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, (_internalTokenSource = new()).Token);
            return Task.Delay(delay, linkedTokenSource.Token);
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
