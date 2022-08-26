namespace NetCord.Services;

[AttributeUsage(AttributeTargets.Parameter)]
public class ParameterAttribute : Attribute
{
    public string? Name { get; }
    public string? Description { get; }

    public ParameterAttribute(string? name, string? description)
    {
        Name = name;
        Description = description;
    }
}
