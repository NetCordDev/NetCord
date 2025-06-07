using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="name">Name of the command (1-32 characters).</param>
/// <param name="description">Description of the command (1-100 characters).</param>
/// <param name="handler">Determines whether the interaction is handled by the app's interactions handler or by Discord.</param>
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
