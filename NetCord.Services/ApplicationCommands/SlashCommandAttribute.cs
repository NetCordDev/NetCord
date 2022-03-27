namespace NetCord.Services.ApplicationCommands;

public class SlashCommandAttribute : ApplicationCommandAttribute
{
    public SlashCommandAttribute(string name, string description) : base(name)
    {
        Description = description;
    }

    public string Description { get; }
    public Type? DescriptionTranslationsProviderType { get; init; }
}