namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Method)]
public abstract class ApplicationCommandAttribute : Attribute
{
    private protected ApplicationCommandAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public Type? NameTranslateProviderType { get; init; }
    public bool DefaultPermission { get; init; } = true;
    public ulong GuildId { get; init; }
    public ulong[]? AllowedRoleIds { get; init; }
    public ulong[]? DisallowedRoleIds { get; init; }
    public ulong[]? AllowedUserIds { get; init; }
    public ulong[]? DisallowedUserIds { get; init; }
}