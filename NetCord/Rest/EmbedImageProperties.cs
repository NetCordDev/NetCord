using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="EmbedImage"/>
/// <param name="url"><inheritdoc cref="Url" path="/summary"/></param>
[GenerateMethodsForProperties]
public partial class EmbedImageProperties(string? url)
{
    /// <inheritdoc cref="EmbedImage.Url"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; } = url;

    public static implicit operator EmbedImageProperties(string? url) => new(url);
}
