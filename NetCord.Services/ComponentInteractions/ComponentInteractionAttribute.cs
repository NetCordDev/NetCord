namespace NetCord.Services.ComponentInteractions;

/// <summary>
/// Component interactions are interactions that are triggered by user actions on components, such as buttons, select menus, and modals.
/// </summary>
/// <param name="customId"><inheritdoc cref="CustomId" path="/summary" /></param>
[AttributeUsage(AttributeTargets.Method)]
public class ComponentInteractionAttribute(string customId) : Attribute
{
    /// <summary>
    /// The custom identifier for the component interaction (0-100 characters).
    /// </summary>
    public string CustomId { get; } = customId;
}
