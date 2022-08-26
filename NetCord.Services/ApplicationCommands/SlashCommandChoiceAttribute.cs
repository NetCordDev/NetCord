namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandChoiceAttribute : Attribute
{
    public string? Name { get; init; }

    public Type? TranslationsProviderType { get; init; }
}
