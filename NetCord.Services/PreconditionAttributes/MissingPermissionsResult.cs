namespace NetCord.Services;

public class MissingPermissionsResult : PreconditionFailResult
{
    public Permissions MissingPermissions { get; }
    public MissingPermissionsResultEntityType EntityType { get; }
    public MissingPermissionsResultPermissionType PermissionType { get; }

    public MissingPermissionsResult(string message, Permissions missingPermissions, MissingPermissionsResultEntityType entityType, MissingPermissionsResultPermissionType permissionType) : base(message)
    {
        MissingPermissions = missingPermissions;
        EntityType = entityType;
        PermissionType = permissionType;
    }
}

public enum MissingPermissionsResultEntityType
{
    User,
    Bot,
}

public enum MissingPermissionsResultPermissionType
{
    Guild,
    Channel,
}
