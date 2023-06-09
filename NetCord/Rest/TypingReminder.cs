namespace NetCord.Rest;

internal class TypingReminder : IDisposable
{
    public ulong ChannelId { get; }

    private readonly RestClient _client;
    private readonly RequestProperties? _properties;
    private readonly CancellationTokenSource _tokenSource;


    public TypingReminder(ulong channelId, RestClient client, RequestProperties? properties)
    {
        ChannelId = channelId;
        _client = client;
        _properties = properties;
        _tokenSource = new();
    }

    public async Task StartAsync()
    {
        await _client.TriggerTypingStateAsync(ChannelId).ConfigureAwait(false);
        _ = RunAsync();
    }

    private async Task RunAsync()
    {
        var token = _tokenSource.Token;
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(9.5));
        while (true)
        {
            await timer.WaitForNextTickAsync(token).ConfigureAwait(false);
            await _client.TriggerTypingStateAsync(ChannelId, _properties).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}
