namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Method)]
public class SlashCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public bool DefaultPermission { get; init; } = true;
    public ulong GuildId { get; init; }
    public ulong[]? AllowedRolesIds { get; init; }
    public ulong[]? DisallowedRolesIds { get; init; }
    public ulong[]? AllowedUsersIds { get; init; }
    public ulong[]? DisallowedUsersIds { get; init; }
    public Permission RequiredBotPermissions { get; init; }
    public Permission RequiredBotChannelPermissions { get; init; }
    public Permission RequiredUserPermissions { get; init; }
    public Permission RequiredUserChannelPermissions { get; init; }

    public SlashCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}