namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Method)]
public class InteractionAttribute : Attribute
{
    public string CustomId { get; }

    public InteractionAttribute(string customId)
    {
        CustomId = customId;
    }
}