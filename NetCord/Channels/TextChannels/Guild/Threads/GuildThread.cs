namespace NetCord;

public abstract class GuildThread : TextGuildChannel
{
    public ThreadMetadata Metadata { get; }
    public ThreadSelfUser? CurrentUser { get; }
    //public int DefaultAutoArchiveDuration => (int)_jsonEntity.DefaultAutoArchiveDuration!;
    public Snowflake OwnerId => _jsonEntity.OwnerId.GetValueOrDefault();
    public override int Position => throw new NotImplementedException($"Threads don't have {nameof(Position)}");

    public Task<IReadOnlyDictionary<Snowflake, ThreadUser>> GetUsersAsync() => _client.GetGuildThreadUsersAsync(Id);

    internal GuildThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        Metadata = new(jsonEntity.Metadata);
        if (jsonEntity.CurrentUser != null)
            CurrentUser = new(jsonEntity.CurrentUser);
    }

    public async Task<GuildThread> ModifyAsync(Action<ThreadOptions> action, RequestProperties? options = null) => (GuildThread)await _client.ModifyGuildThreadAsync(Id, action, options).ConfigureAwait(false);
}