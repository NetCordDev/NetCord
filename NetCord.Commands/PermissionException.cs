namespace NetCord.Commands
{
    public class PermissionException : Exception
    {
        public Permission MissingPermissions { get; }

        internal PermissionException(string message, Permission missingPermissions) : base(message)
        {
            MissingPermissions = missingPermissions;
        }
    }
}