namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    public string? Name { get; init; }

    public Type? NameTranslationsProviderType { get; init; }

    public string? Description { get; init; }

    public Type? DescriptionTranslationsProviderType { get; init; }

    public ChannelType[]? AllowedChannelTypes { get; init; }

    public Type? ChoicesProviderType { get; init; }

    public Type? AutocompleteProviderType { get; init; }
}