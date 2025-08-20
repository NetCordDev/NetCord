using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Url of the image.
/// </summary>
/// <param name="url"></param>
[GenerateMethodsForProperties]
public partial class EmbedImageProperties(string? url)
{
    /// <summary>
    /// Url of the image.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; } = url;

    public static implicit operator EmbedImageProperties(string? url) => new(url);
}
