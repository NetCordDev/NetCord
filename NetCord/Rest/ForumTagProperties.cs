using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ForumTagProperties(string name)
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// The name of the tag. (0-20 characters)
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    /// <summary>
    /// Whether this tag can only be added to or removed from threads by a member with the MANAGE_THREADS permission.
    /// </summary>
    /// <remarks>
    /// This is not required when updating a forum or media channel. Otherwise must be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("moderated")]
    public bool? Moderated { get; set; }

    /// <summary>
    /// The ID of the guild's custom emoji to display for this tag.
    /// </summary>
    /// <remarks>
    /// At most one of <see cref="EmojiId"/> and <see cref="EmojiName"/> may be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    /// <summary>
    /// The unicode character of the standard emoji to display for this tag.
    /// </summary>
    /// <remarks>
    /// At most one of <see cref="EmojiId"/> and <see cref="EmojiName"/> may be non-null.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
}
