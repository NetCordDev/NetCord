namespace NetCord;

public class PermissionOverwrite : ClientEntity
{
    private readonly JsonModels.JsonPermissionOverwrite _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public PermissionOverwriteType Type => _jsonEntity.Type;

    public string Allowed => _jsonEntity.Allowed;

    public string Denied => _jsonEntity.Denied;

    internal PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }
}

public enum PermissionOverwriteType
{
    Role = 0,
    User = 1,
}
