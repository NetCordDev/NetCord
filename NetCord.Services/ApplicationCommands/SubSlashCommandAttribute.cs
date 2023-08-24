namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SubSlashCommandAttribute : Attribute
{
    public SubSlashCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public Type? NameTranslationsProviderType { get; init; }

    public string Description { get; }

    public Type? DescriptionTranslationsProviderType { get; init; }
}
