using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ForumTagProperties(string name)
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    /// <remarks>
    /// This is not required when updating a forum or media channel. Otherwise must be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    /// <remarks>
    /// The maximum length is 20 characters.
    /// </remarks>
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    /// <summary>
    /// Whether the tag is moderated.
    /// </summary>
    /// <remarks>
    /// This is not required when updating a forum or media channel. Otherwise must be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("moderated")]
    public bool? Moderated { get; set; }

    /// <summary>
    /// The ID of the emoji to display for this tag.
    /// </summary>
    /// <remarks>
    /// This is not required when updating a forum or media channel. Otherwise must be non-null
    /// At most one of <see cref="EmojiId"/> and <see cref="EmojiName"/> may be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    /// <summary>
    /// The name of the emoji to display for this tag.
    /// </summary>
    /// <remarks>
    /// This is not required when updating a forum or media channel. Otherwise must be non-null
    /// At most one of <see cref="EmojiId"/> and <see cref="EmojiName"/> may be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
}
