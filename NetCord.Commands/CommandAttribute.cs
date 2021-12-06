namespace NetCord.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string[] Aliases { get; }
        public int Priority { get; init; }
        public Context RequiredContext { get; init; }
        public Permission RequiredBotPermissions { get; init; }
        public Permission RequiredBotChannelPermissions { get; init; }
        public Permission RequiredUserPermissions { get; init; }
        public Permission RequiredUserChannelPermissions { get; init; }

        public CommandAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}