using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="url">Url of the thumbnail.</param>
public partial class EmbedThumbnailProperties(string? url)
{
    /// <summary>
    /// Url of the thumbnail.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; } = url;

    public static implicit operator EmbedThumbnailProperties(string? url) => new(url);
}
