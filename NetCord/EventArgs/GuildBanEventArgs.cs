using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildBanEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildBanEventArgs _jsonEntity;

    internal GuildBanEventArgs(JsonGuildBanEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(_jsonEntity.User, client);
    }

    public Snowflake GuildId => _jsonEntity.GuildId;

    public User User { get; }
}
