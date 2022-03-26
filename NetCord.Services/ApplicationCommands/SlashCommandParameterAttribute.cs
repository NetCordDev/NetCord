namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    public string? Name { get; init; }

    public Type? NameTranslateProviderType { get; init; }

    public string? Description { get; init; }

    public Type? DescriptionTranslateProviderType { get; init; }

    public ChannelType[]? AllowedChannelTypes { get; init; }

    public Type? ChoicesProviderType { get; init; }

    public Type? AutocompleteProviderType { get; init; }
}