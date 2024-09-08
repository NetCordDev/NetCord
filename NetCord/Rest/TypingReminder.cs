namespace NetCord.Rest;

internal sealed class TypingReminder(ulong channelId, RestClient client, RestRequestProperties? properties) : IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await client.TriggerTypingStateAsync(channelId, properties, cancellationToken).ConfigureAwait(false);
        _ = RunAsync();
    }

    private async Task RunAsync()
    {
        var cancellationToken = _tokenSource.Token;
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(9.5));
        while (true)
        {
            await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
            await client.TriggerTypingStateAsync(channelId, properties, cancellationToken).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }
}
