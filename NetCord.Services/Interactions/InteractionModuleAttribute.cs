namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Class)]
public class InteractionModuleAttribute : Attribute
{
    public Permission RequiredBotPermissions { get; init; }
    public Permission RequiredBotChannelPermissions { get; init; }
    public Permission RequiredUserPermissions { get; init; }
    public Permission RequiredUserChannelPermissions { get; init; }
}