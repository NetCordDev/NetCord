namespace NetCord.Services.Commands;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string[] Aliases { get; }
    public int Priority { get; init; }

    public CommandAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}
