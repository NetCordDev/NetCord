using NetCord.JsonModels;

namespace NetCord;

internal class Ready
{
    private readonly JsonReady _jsonEntity;

    public SelfUser User { get; }

    public string SessionId => _jsonEntity.SessionId;

    public Application Application { get; }

    public IEnumerable<DMChannel> DMChannels { get; }

    internal Ready(JsonReady jsonEntity, BotClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(jsonEntity.User, client);
        Application = new(jsonEntity.Application, client);
        DMChannels = jsonEntity.DMChannels.Select(c => (DMChannel)Channel.CreateFromJson(c, client));
    }
}