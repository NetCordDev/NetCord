using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class CurrentGuildUserOptions
{
    internal CurrentGuildUserOptions()
    {
    }

    /// <summary>
    /// New nickname, empty to remove nickname.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }

    /// <summary>
    /// New banner image.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("banner")]
    public ImageProperties? Banner { get; set; }

    /// <summary>
    /// New avatar image.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }

    /// <summary>
    /// New bio, empty to remove bio.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bio")]
    public string? Bio { get; set; }
}
