namespace NetCord.Commands;

[AttributeUsage(AttributeTargets.Parameter)]
public class CommandParameterAttribute : Attribute
{
    public string? Name { get; }
    public string? Description { get; }

    public CommandParameterAttribute(string? name, string? description)
    {
        Name = name;
        Description = description;
    }
}