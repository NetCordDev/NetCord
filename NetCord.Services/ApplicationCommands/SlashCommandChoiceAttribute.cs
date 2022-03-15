namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandChoiceAttribute : Attribute
{
    public string Name { get; }

    public SlashCommandChoiceAttribute(string name)
    {
        Name = name;
    }
}