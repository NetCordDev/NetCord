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

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.IntegrationTypes" />
    public ApplicationIntegrationType[]? IntegrationTypes { get; init; }

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.Contexts" />
    public InteractionContextType[]? Contexts { get; init; }

    /// <inheritdoc cref="Rest.ApplicationCommandProperties.Nsfw" />
    public bool Nsfw { get; init; }

    /// <summary>
    /// Whether the application command should be registered by the service.
    /// </summary>
    public bool Register { get; init; } = true;
}
