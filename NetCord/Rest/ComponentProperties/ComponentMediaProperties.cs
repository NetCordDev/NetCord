using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ComponentMediaProperties(string url)
{
    /// <summary>
    /// Source URL of the media item.
    /// </summary>
    /// <remarks>
    /// Supports arbitrary urls and attachment://&lt;filename&gt; references.
    /// For a file component, only supports using the attachment:// protocol.
    /// </remarks>
    [JsonPropertyName("url")]
    public string Url { get; set; } = url;

    public static implicit operator ComponentMediaProperties(string url) => new(url);
}
