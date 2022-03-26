namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandChoiceAttribute : Attribute
{
    public string? Name { get; init; }

    public Type? TranslateProviderType { get; init; }
}