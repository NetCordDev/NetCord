namespace NetCord.Services.Commands;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute(params string[] aliases) : Attribute
{
    public string[] Aliases { get; } = aliases;
    public int Priority { get; init; }
}
