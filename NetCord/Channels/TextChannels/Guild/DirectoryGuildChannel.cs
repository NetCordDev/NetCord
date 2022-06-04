using NetCord.JsonModels;

namespace NetCord;

public class DirectoryGuildChannel : Channel, IGuildChannel
{
    internal DirectoryGuildChannel(JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        PermissionOverwrites = jsonEntity.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public string Name => _jsonEntity.Name!;

    public int Position => _jsonEntity.Position.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, options).ConfigureAwait(false);
}