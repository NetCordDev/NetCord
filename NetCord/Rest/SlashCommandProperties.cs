using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Slash commands are application commands that are invoked by typing a slash (/) in the chat input box.
/// They allow users to interact with your application.
/// </summary>
/// <param name="name"><inheritdoc cref="ApplicationCommandProperties.Name" path="/summary" /> Must be lowercase.</param>
/// <param name="description"><inheritdoc cref="Description" path="/summary" /></param>
public partial class SlashCommandProperties(string name, string description) : ApplicationCommandProperties(ApplicationCommandType.ChatInput, name)
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
    /// Parameters for the command (max 25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    public override void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.SlashCommandProperties);
    }
}
