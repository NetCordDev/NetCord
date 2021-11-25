namespace NetCord.Commands
{
    public class PermissionException : Exception
    {
        public PermissionFlags MissingPermissions { get; }

        internal PermissionException(string message, PermissionFlags missingPermissions) : base(message)
        {
            MissingPermissions = missingPermissions;
        }
    }
}