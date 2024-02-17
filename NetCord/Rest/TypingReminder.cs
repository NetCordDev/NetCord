namespace NetCord.Rest;

internal sealed class TypingReminder(ulong channelId, RestClient client, RequestProperties? properties) : IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();

    public async Task StartAsync()
    {
        await client.TriggerTypingStateAsync(channelId).ConfigureAwait(false);
        _ = RunAsync();
    }

    private async Task RunAsync()
    {
        var token = _tokenSource.Token;
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(9.5));
        while (true)
        {
            await timer.WaitForNextTickAsync(token).ConfigureAwait(false);
            await client.TriggerTypingStateAsync(channelId, properties).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }
}
