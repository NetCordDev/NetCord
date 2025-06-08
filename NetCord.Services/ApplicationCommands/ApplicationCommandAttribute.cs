namespace NetCord.Services.ApplicationCommands;

/// <inheritdoc cref="Rest.ApplicationCommandProperties" />
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class ApplicationCommandAttribute : Attribute
{
    private protected ApplicationCommandAttribute(string name)
    {
        Name = name;
    }

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.Name" />
    public string Name { get; }

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.DefaultGuildUserPermissions" />
    public Permissions DefaultGuildUserPermissions
    {
        get => _defaultGuildUserPermissions.GetValueOrDefault();
        init
        {
            _defaultGuildUserPermissions = value;
        }
    }

    internal readonly Permissions? _defaultGuildUserPermissions;

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.DMPermission" />
    [Obsolete($"Replaced by '{nameof(Contexts)}'.")]
    public bool DMPermission
    {
        get => _dMPermission.GetValueOrDefault();
        init
        {
            _dMPermission = value;
        }
    }

    internal readonly bool? _dMPermission;

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.DefaultPermission" />
    [Obsolete($"Replaced by '{nameof(DefaultGuildUserPermissions)}'.")]
    public bool DefaultPermission { get; init; } = true;

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.IntegrationTypes" />
    public ApplicationIntegrationType[]? IntegrationTypes { get; init; }

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.Contexts" />
    public InteractionContextType[]? Contexts { get; init; }

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.Nsfw" />
    public bool Nsfw { get; init; }

    /// <summary>
    /// The ID of the guild where the application command is registered.
    /// </summary>
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
