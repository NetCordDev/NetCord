namespace NetCord;

public class PermissionOverwrite : Entity
{
    private readonly JsonModels.JsonPermissionOverwrite _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public PermissionOverwriteType Type => _jsonEntity.Type;

    public PermissionFlags Allowed { get; }

    public PermissionFlags Denied { get; }

    internal PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Allowed = (PermissionFlags)ulong.Parse(jsonEntity.Allowed);
        Denied = (PermissionFlags)ulong.Parse(jsonEntity.Denied);
    }
}

public enum PermissionOverwriteType
{
    Role = 0,
    User = 1,
}
