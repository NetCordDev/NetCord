namespace NetCord;

public class PermissionOverwrite : Entity
{
    private readonly JsonModels.JsonPermissionOverwrite _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public PermissionOverwriteType Type => _jsonEntity.Type;

    public Permission Allowed { get; }

    public Permission Denied { get; }

    internal PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Allowed = (Permission)ulong.Parse(jsonEntity.Allowed);
        Denied = (Permission)ulong.Parse(jsonEntity.Denied);
    }
}

public enum PermissionOverwriteType
{
    Role = 0,
    User = 1,
}
