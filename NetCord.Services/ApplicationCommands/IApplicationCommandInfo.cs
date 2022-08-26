using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandInfo
{
    public Type DeclaringType { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool DefaultPermission { get; }
    public Snowflake? GuildId { get; }
    public Func<object, object?[]?, Task> InvokeAsync { get; }
    public Dictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public ApplicationCommandType Type { get; }

    public ApplicationCommandProperties GetRawValue();
}
