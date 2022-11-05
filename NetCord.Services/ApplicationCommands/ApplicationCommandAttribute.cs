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

    public Permission DefaultGuildUserPermissions
    {
        get => _defaultGuildUserPermissions.GetValueOrDefault();
        init
        {
            _defaultGuildUserPermissions = value;
        }
    }

    internal readonly Permission? _defaultGuildUserPermissions;

    public bool DMPermission
    {
        get => _dMPermission.GetValueOrDefault();
        init
        {
            _dMPermission = value;
        }
    }

    internal readonly bool? _dMPermission;

    [Obsolete("Replaced by `default_member_permissions`")]
    public bool DefaultPermission { get; init; } = true;

    public ulong GuildId { get; init; }
}
