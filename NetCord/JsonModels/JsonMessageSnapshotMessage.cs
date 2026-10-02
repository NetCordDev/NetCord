using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageSnapshotMessage
{
    [JsonPropertyName("type")]
    public required MessageType Type { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("embeds")]
    public required JsonEmbed[] Embeds { get; set; }

    [JsonPropertyName("attachments")]
    public required JsonAttachment[] Attachments { get; set; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedAt { get; set; }

    [JsonPropertyName("flags")]
    public required MessageFlags? Flags { get; set; }

    [JsonPropertyName("mentions")]
    public required JsonUser[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_roles")]
    public required ulong[] MentionedRoleIds { get; set; }

    [JsonPropertyName("sticker_items")]
    public JsonMessageSticker[]? Stickers { get; set; }

    [JsonPropertyName("components")]
    public JsonComponent[]? Components { get; set; }
}
