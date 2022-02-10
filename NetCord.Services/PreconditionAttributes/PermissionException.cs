namespace NetCord.Services;

public class PermissionException : Exception
{
    public Permission MissingPermissions { get; }
    public PermissionExceptionEntityType EntityType { get; }
    public PermissionExceptionPermissionType PermissionType { get; }

    public PermissionException(string? message, Permission missingPermissions, PermissionExceptionEntityType entityType, PermissionExceptionPermissionType permissionType) : base(message)
    {
        MissingPermissions = missingPermissions;
        EntityType = entityType;
        PermissionType = permissionType;
    }
}

public enum PermissionExceptionEntityType
{
    User,
    Bot,
}

public enum PermissionExceptionPermissionType
{
    Guild,
    Channel,
}