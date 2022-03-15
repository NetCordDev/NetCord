namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public Type DeclaringType { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool DefaultPermission { get; init; }
    public DiscordId? GuildId { get; init; }
    public IEnumerable<DiscordId>? AllowedRoleIds { get; init; }
    public IEnumerable<DiscordId>? DisallowedRoleIds { get; init; }
    public IEnumerable<DiscordId>? AllowedUserIds { get; init; }
    public IEnumerable<DiscordId>? DisallowedUserIds { get; init; }
    public Func<object, object?[]?, Task> InvokeAsync { get; }
    public Dictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public ApplicationCommandType Type { get; }

    public ApplicationCommandProperties GetRawValue();

    public IEnumerable<ApplicationCommandPermissionProperties> GetRawPermissions();
}