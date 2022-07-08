using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class DirectoryGuildChannel : Channel, IGuildChannel
{
    public DirectoryGuildChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public string Name => _jsonModel.Name!;

    public int Position => _jsonModel.Position.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
}