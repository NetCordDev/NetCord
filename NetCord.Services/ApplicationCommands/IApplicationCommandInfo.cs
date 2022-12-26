using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public Type DeclaringType { get; }
    public bool Static { get; }
    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string? Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }
    public Func<object?[], Task> InvokeAsync { get; }
    public IReadOnlyDictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public ApplicationCommandType Type { get; }

    public ApplicationCommandProperties GetRawValue();
}
