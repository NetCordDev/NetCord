namespace NetCord;

public abstract class Thread : TextGuildChannel
{
    public ThreadMetadata Metadata { get; }
    public ThreadSelfUser? CurrentUser { get; }
    //public int DefaultAutoArchiveDuration => (int)_jsonEntity.DefaultAutoArchiveDuration!;
    public DiscordId OwnerId => _jsonEntity.OwnerId.GetValueOrDefault();
    public override int Position => throw new NotImplementedException($"Threads don't have {nameof(Position)}");

    public Task<IReadOnlyDictionary<DiscordId, ThreadUser>> GetUsersAsync() => _client.Guild.Channel.GetThreadUsersAsync(Id);

    internal Thread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        Metadata = new(jsonEntity.Metadata);
        if (jsonEntity.CurrentUser != null)
            CurrentUser = new(jsonEntity.CurrentUser);
    }

    public async Task<Thread> ModifyAsync(Action<ThreadOptions> action, RequestOptions? options = null) => (Thread)await _client.Guild.Channel.ModifyAsync(Id, action, options).ConfigureAwait(false);
}