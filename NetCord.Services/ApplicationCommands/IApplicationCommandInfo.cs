using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public ApplicationCommandType Type { get; }

    public string Name { get; }

    public ILocalizationsProvider? LocalizationsProvider { get; }

    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }

    public Permissions? DefaultGuildPermissions { get; }

    public IEnumerable<ApplicationIntegrationType>? IntegrationTypes { get; }

    public IEnumerable<InteractionContextType>? Contexts { get; }

    public bool Nsfw { get; }

    public bool Register { get; }

    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }

    public ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
