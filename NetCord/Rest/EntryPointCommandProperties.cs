using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Entry Point command serves as the primary way for users to open an app's Activity from the App Launcher.
/// You can create only a single Entry Point command per app.
/// </summary>
/// <param name="name"><inheritdoc cref="ApplicationCommandProperties.Name" path="/summary" /></param>
/// <param name="description"><inheritdoc cref="Description" path="/summary" /></param>
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
public partial class EntryPointCommandProperties(string name, string description, EntryPointCommandHandler handler) : ApplicationCommandProperties(ApplicationCommandType.EntryPoint, name)
{
    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    /// <summary>
    /// Localizations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; set; }

    /// <summary>
    /// Determines whether the interaction is handled by the app's interactions handler or by Discord.
    /// </summary>
    [JsonPropertyName("handler")]
    public EntryPointCommandHandler Handler { get; set; } = handler;
}
