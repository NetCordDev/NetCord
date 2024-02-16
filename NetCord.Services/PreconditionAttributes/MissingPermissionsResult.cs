namespace NetCord.Services;

public class MissingPermissionsResult(string message, Permissions missingPermissions, MissingPermissionsResultEntityType entityType, MissingPermissionsResultPermissionType permissionType) : PreconditionFailResult(message)
{
    public Permissions MissingPermissions { get; } = missingPermissions;
    public MissingPermissionsResultEntityType EntityType { get; } = entityType;
    public MissingPermissionsResultPermissionType PermissionType { get; } = permissionType;
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
