using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class SlashCommandProperties : ApplicationCommandProperties
{
    /// <summary>
    /// Description of the command (1-100 characters).
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Translations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    /// <summary>
    /// Parameters for the command (max 25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Name of the command (1-32 characters).</param>
    /// <param name="description">Description of the command (1-100 characters).</param>
    public SlashCommandProperties(string name, string description) : base(ApplicationCommandType.ChatInput, name)
    {
        Description = description;
    }
}
