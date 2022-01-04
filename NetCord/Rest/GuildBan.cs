namespace NetCord;

public class GuildBan
{
    private readonly JsonModels.JsonGuildBan _jsonEntity;

    public string? Reason => _jsonEntity.Reason;

    public User User { get; }

    internal GuildBan(JsonModels.JsonGuildBan jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(jsonEntity.User, client);
    }
}