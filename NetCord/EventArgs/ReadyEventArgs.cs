using NetCord.Gateway;

namespace NetCord;

public class ReadyEventArgs
{
    private readonly JsonModels.EventArgs.JsonReadyEventArgs _jsonEntity;

    public SelfUser User { get; }

    public IEnumerable<Snowflake> GuildIds { get; }

    public string SessionId => _jsonEntity.SessionId;

    public Shard? Shard => _jsonEntity.Shard;

    public Snowflake ApplicationId => _jsonEntity.Application.Id;

    public ApplicationFlags ApplicationFlags => _jsonEntity.Application.Flags.GetValueOrDefault();

    public IEnumerable<DMChannel> DMChannels { get; }

    internal ReadyEventArgs(JsonModels.EventArgs.JsonReadyEventArgs jsonEntity, GatewayClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(jsonEntity.User, client.Rest);
        GuildIds = _jsonEntity.Guilds.Select(g => g.Id);
        DMChannels = _jsonEntity.DMChannels.Select(c => (DMChannel)Channel.CreateFromJson(c, client.Rest));
    }
}