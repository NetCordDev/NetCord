namespace NetCord.Services;

public class PermissionException : Exception
{
    public Permission MissingPermissions { get; }

    public PermissionException(string? message, Permission missingPermissions) : base(message)
    {
        MissingPermissions = missingPermissions;
    }
}