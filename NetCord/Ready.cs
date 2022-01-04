using NetCord.JsonModels;

namespace NetCord;

internal class Ready
{
    private readonly JsonReady _jsonEntity;

    public SelfUser User { get; }

    public string SessionId => _jsonEntity.SessionId;

    public Application? Application { get; }

    public IEnumerable<DMChannel> DMChannels { get; }

    internal Ready(JsonReady jsonEntity, SocketClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(jsonEntity.User, client.Rest);
        if (jsonEntity.Application != null)
            Application = new(jsonEntity.Application, client.Rest);
        DMChannels = jsonEntity.DMChannels.Select(c => (DMChannel)Channel.CreateFromJson(c, client.Rest));
    }
}