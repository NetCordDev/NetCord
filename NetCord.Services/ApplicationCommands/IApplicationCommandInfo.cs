using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    /// <summary>
    /// The type of the application command.
    /// </summary>
    public abstract ApplicationCommandType Type { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Name" />
    public string Name { get; }

    /// <summary>
    /// The provider for localizations of the command.
    /// </summary>
    public ILocalizationsProvider? LocalizationsProvider { get; }

    /// <summary>
    /// The localization path for the command.
    /// </summary>
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.DefaultGuildPermissions" />
    public Permissions? DefaultGuildPermissions { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.IntegrationTypes" />
    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Contexts" />
    public IEnumerable<InteractionContextType>? Contexts { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Nsfw" />
    public bool Nsfw { get; }

    /// <inheritdoc cref="ApplicationCommandAttribute.Register" />
    public bool Register { get; }

    /// <summary>
    /// Gets the raw value of the command that can be used to create it.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ApplicationCommandProperties"/>.</returns>
    public ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
