using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public Type DeclaringType { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool DefaultPermission { get; init; }
    public Snowflake? GuildId { get; init; }
    public IEnumerable<Snowflake>? AllowedRoleIds { get; init; }
    public IEnumerable<Snowflake>? DisallowedRoleIds { get; init; }
    public IEnumerable<Snowflake>? AllowedUserIds { get; init; }
    public IEnumerable<Snowflake>? DisallowedUserIds { get; init; }
    public Func<object, object?[]?, Task> InvokeAsync { get; }
    public Dictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public ApplicationCommandType Type { get; }

    public ApplicationCommandProperties GetRawValue();
}