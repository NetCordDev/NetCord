namespace NetCord;

public abstract class GuildThread : TextGuildChannel
{
    public ThreadMetadata Metadata { get; }
    public ThreadSelfUser? CurrentUser { get; }
    //public int DefaultAutoArchiveDuration => (int)_jsonModel.DefaultAutoArchiveDuration!;
    public Snowflake OwnerId => _jsonModel.OwnerId.GetValueOrDefault();
    public override int Position => throw new InvalidOperationException($"Threads don't have {nameof(Position)}");

    public Task<IReadOnlyDictionary<Snowflake, ThreadUser>> GetUsersAsync() => _client.GetGuildThreadUsersAsync(Id);

    public GuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Metadata = new(jsonModel.Metadata);
        if (jsonModel.CurrentUser != null)
            CurrentUser = new(jsonModel.CurrentUser);
    }

    public async Task<GuildThread> ModifyAsync(Action<ThreadOptions> action, RequestProperties? options = null) => (GuildThread)await _client.ModifyGuildThreadAsync(Id, action, options).ConfigureAwait(false);
}