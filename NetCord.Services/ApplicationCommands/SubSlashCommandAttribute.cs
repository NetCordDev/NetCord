namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Sub slash command allowing to create nested slash commands within a slash command.
/// </summary>
/// <param name="name"><inheritdoc cref="Name" path="/summary" /></param>
/// <param name="description"><inheritdoc cref="Description" path="/summary" /></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SubSlashCommandAttribute(string name, string description) : Attribute
{
    /// <summary>
    /// Name of the sub slash command (1-32 characters).
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Description of the sub slash command (1-100 characters).
    /// </summary>
    public string Description { get; } = description;
}
