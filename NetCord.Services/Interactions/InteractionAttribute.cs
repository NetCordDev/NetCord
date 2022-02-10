namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Method)]
public class InteractionAttribute : Attribute
{
    public string Alias { get; }

    public InteractionAttribute(string alias)
    {
        Alias = alias;
    }
}