using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="EmbedThumbnail"/>
/// <param name="url"><inheritdoc cref="Url" path="/summary"/></param>
[GenerateMethodsForProperties]
public partial class EmbedThumbnailProperties(string? url)
{
    /// <inheritdoc cref="EmbedThumbnail.Url"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; } = url;

    public static implicit operator EmbedThumbnailProperties(string? url) => new(url);
}
