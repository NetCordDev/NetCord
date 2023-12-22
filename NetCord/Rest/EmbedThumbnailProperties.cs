using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class EmbedThumbnailProperties
{
    /// <summary>
    /// Url of the thumbnail.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the thumbnail.</param>
    public EmbedThumbnailProperties(string? url)
    {
        Url = url;
    }

    public static implicit operator EmbedThumbnailProperties(string? url) => new(url);
}
