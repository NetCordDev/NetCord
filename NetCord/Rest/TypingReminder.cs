namespace NetCord.Rest;

internal class TypingReminder(ulong channelId, RestClient client, RequestProperties? properties) : IDisposable
{
    public ulong ChannelId { get; } = channelId;

    private readonly CancellationTokenSource _tokenSource = new();

    public async Task StartAsync()
    {
        await client.TriggerTypingStateAsync(ChannelId).ConfigureAwait(false);
        _ = RunAsync();
    }

    private async Task RunAsync()
    {
        var token = _tokenSource.Token;
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(9.5));
        while (true)
        {
            await timer.WaitForNextTickAsync(token).ConfigureAwait(false);
            await client.TriggerTypingStateAsync(ChannelId, properties).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}
