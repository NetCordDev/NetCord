using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }

    public ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
