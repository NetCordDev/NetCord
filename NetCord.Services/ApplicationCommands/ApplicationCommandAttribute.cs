namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Method)]
public abstract class ApplicationCommandAttribute : Attribute
{
    private protected ApplicationCommandAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public Type? NameTranslationsProviderType { get; init; }
    public Permission DefaultGuildUserPermissions { get; init; } = (Permission)((ulong)1 << 63);
    public bool DMPermission { get; init; } = false;

    [Obsolete("Replaced by `default_member_permissions`")]
    public bool DefaultPermission { get; init; } = true;
    public ulong GuildId { get; init; }
}