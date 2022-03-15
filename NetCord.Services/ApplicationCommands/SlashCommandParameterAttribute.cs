namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    public string? Name { get; }

    public string Description { get; }

    public SlashCommandParameterAttribute(string? name, string description) : this(description)
    {
        Name = name;
    }

    public SlashCommandParameterAttribute(string description)
    {
        Description = description;
    }
}