using NetCord.Gateway;

namespace NetCord;

public class ReadyEventArgs : IJsonModel<JsonModels.EventArgs.JsonReadyEventArgs>
{
    JsonModels.EventArgs.JsonReadyEventArgs IJsonModel<JsonModels.EventArgs.JsonReadyEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonReadyEventArgs _jsonModel;

    public SelfUser User { get; }

    public IEnumerable<Snowflake> GuildIds { get; }

    public string SessionId => _jsonModel.SessionId;

    public Shard? Shard => _jsonModel.Shard;

    public Snowflake ApplicationId => _jsonModel.Application.Id;

    public ApplicationFlags ApplicationFlags => _jsonModel.Application.Flags.GetValueOrDefault();

    public IEnumerable<DMChannel> DMChannels { get; }

    public ReadyEventArgs(JsonModels.EventArgs.JsonReadyEventArgs jsonModel, GatewayClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client.Rest);
        GuildIds = _jsonModel.Guilds.Select(g => g.Id);
        DMChannels = _jsonModel.DMChannels.Select(c => (DMChannel)Channel.CreateFromJson(c, client.Rest));
    }
}