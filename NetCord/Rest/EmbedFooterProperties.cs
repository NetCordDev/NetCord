using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="EmbedFooter"/>
[GenerateMethodsForProperties]
public partial class EmbedFooterProperties
{

    /// <inheritdoc cref="EmbedFooter.Text"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Points to an image, which is displayed in a small circular format to the left of the <see cref="Text"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}
