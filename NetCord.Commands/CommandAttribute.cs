namespace NetCord.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string[] Aliases { get; }
        public int Priority { get; init; }
        public Context RequiredContext { get; init; }
        public PermissionFlags RequiredBotPermissions { get; init; }
        public PermissionFlags RequiredBotChannelPermissions { get; init; }
        public PermissionFlags RequiredUserPermissions { get; init; }
        public PermissionFlags RequiredUserChannelPermissions { get; init; }

        public CommandAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}