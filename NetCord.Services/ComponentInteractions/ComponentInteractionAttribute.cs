namespace NetCord.Services.ComponentInteractions;

[AttributeUsage(AttributeTargets.Method)]
public class ComponentInteractionAttribute(string customId) : Attribute
{
    public string CustomId { get; } = customId;
}
