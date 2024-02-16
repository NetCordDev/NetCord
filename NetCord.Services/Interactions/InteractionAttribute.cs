namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Method)]
public class InteractionAttribute(string customId) : Attribute
{
    public string CustomId { get; } = customId;
}
