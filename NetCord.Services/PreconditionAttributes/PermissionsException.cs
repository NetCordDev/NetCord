using System.Runtime.Serialization;

namespace NetCord.Services;

[Serializable]
public class PermissionsException : Exception
{
    public Permissions MissingPermissions { get; }
    public PermissionsExceptionEntityType EntityType { get; }
    public PermissionsExceptionPermissionType PermissionType { get; }

    public PermissionsException(string? message, Permissions missingPermissions, PermissionsExceptionEntityType entityType, PermissionsExceptionPermissionType permissionType) : base(message)
    {
        MissingPermissions = missingPermissions;
        EntityType = entityType;
        PermissionType = permissionType;
    }

    protected PermissionsException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        MissingPermissions = (Permissions)serializationInfo.GetUInt64(nameof(MissingPermissions));
        EntityType = (PermissionsExceptionEntityType)serializationInfo.GetInt32(nameof(EntityType));
        PermissionType = (PermissionsExceptionPermissionType)serializationInfo.GetInt32(nameof(PermissionType));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(MissingPermissions), (ulong)MissingPermissions);
        info.AddValue(nameof(EntityType), (int)EntityType);
        info.AddValue(nameof(PermissionType), (int)PermissionType);
    }
}

public enum PermissionsExceptionEntityType
{
    User,
    Bot,
}

public enum PermissionsExceptionPermissionType
{
    Guild,
    Channel,
}
