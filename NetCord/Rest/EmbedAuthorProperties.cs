using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="EmbedAuthor"/>
[GenerateMethodsForProperties]
public partial class EmbedAuthorProperties
{
    /// <inheritdoc cref="EmbedAuthor.Name"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <inheritdoc cref="EmbedAuthor.Url"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <inheritdoc cref="EmbedAuthor.IconUrl"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}
