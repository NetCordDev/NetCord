using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents a modification to perform on a <see cref="GuildSticker"/>.
/// </summary>
[GenerateMethodsForProperties]
public partial class GuildStickerOptions
{
    internal GuildStickerOptions()
    {
    }

    /// <inheritdoc cref="Sticker.Name"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <inheritdoc cref="Sticker.Description"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// A comma separated-list of the sticker's autocomplete/suggestion tags.
    /// </summary>
    /// <remarks>
    /// The string's character count cannot exceed 200.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tags")]
    public string? Tags { get; set; }
}
