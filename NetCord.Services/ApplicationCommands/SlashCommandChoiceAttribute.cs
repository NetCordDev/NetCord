namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandChoiceAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
