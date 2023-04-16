using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }
    public ApplicationCommandType Type { get; }
    public string? Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IReadOnlyDictionary<string, Delegate>? Autocompletes { get; }

    public ApplicationCommandProperties GetRawValue();
}
