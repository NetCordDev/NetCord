namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Method)]
public class InteractionAttribute : Attribute
{
    public string Alias { get; }
    public Permission RequiredBotPermissions { get; init; }
    public Permission RequiredBotChannelPermissions { get; init; }
    public Permission RequiredUserPermissions { get; init; }
    public Permission RequiredUserChannelPermissions { get; init; }

    public InteractionAttribute(string alias)
    {
        Alias = alias;
    }
}