namespace NetCord;

internal class TypingReminder : IDisposable
{
    private readonly RestClient _client;

    public Snowflake ChannelId { get; }

    private readonly CancellationTokenSource _tokenSource;
    private readonly RequestProperties? _options;

    public TypingReminder(Snowflake channelId, RestClient client, RequestProperties? options)
    {
        ChannelId = channelId;
        _client = client;
        _options = options;
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
        PeriodicTimer timer = new(TimeSpan.FromSeconds(9.5));
        while (true)
        {
            await timer.WaitForNextTickAsync(token).ConfigureAwait(false);
            await _client.TriggerTypingStateAsync(ChannelId, _options).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}