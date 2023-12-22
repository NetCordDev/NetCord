using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class EmbedFooterProperties
{
    /// <summary>
    /// Text of the footer.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Url of the footer icon.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}
