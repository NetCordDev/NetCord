namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Specifies metadata for a choice of a slash command parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandChoiceAttribute : Attribute
{
    /// <summary>
    /// Name of the choice (1-100 characters).
    /// </summary>
    public string? Name { get; init; }
}
