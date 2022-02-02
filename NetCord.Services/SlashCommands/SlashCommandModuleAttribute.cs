namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Class)]
public class SlashCommandModuleAttribute : Attribute
{
    public Permission RequiredBotPermissions { get; init; }
    public Permission RequiredBotChannelPermissions { get; init; }
    public Permission RequiredUserPermissions { get; init; }
    public Permission RequiredUserChannelPermissions { get; init; }
}