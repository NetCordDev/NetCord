namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class ApplicationCommandAttribute : Attribute
{
    private protected ApplicationCommandAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public Permissions DefaultGuildUserPermissions
    {
        get => _defaultGuildUserPermissions.GetValueOrDefault();
        init
        {
            _defaultGuildUserPermissions = value;
        }
    }

    internal readonly Permissions? _defaultGuildUserPermissions;

    public bool DMPermission
    {
        get => _dMPermission.GetValueOrDefault();
        init
        {
            _dMPermission = value;
        }
    }

    internal readonly bool? _dMPermission;

    [Obsolete("Replaced by 'DefaultGuildUserPermissions'.")]
    public bool DefaultPermission { get; init; } = true;

    public bool Nsfw { get; init; }

    public ulong GuildId
    {
        get => _guildId.GetValueOrDefault();
        init
        {
            _guildId = value;
        }
    }

    internal readonly ulong? _guildId;
}
