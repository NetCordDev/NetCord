using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonMessageSnapshotMessage
{
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("embeds")]
    public JsonEmbed[] Embeds { get; set; }

    [JsonPropertyName("attachments")]
    public JsonAttachment[] Attachments { get; set; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedAt { get; set; }

    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonPropertyName("mentions")]
    public JsonUser[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_roles")]
    public ulong[] MentionedRoleIds { get; set; }

    [JsonPropertyName("sticker_items")]
    public JsonMessageSticker[]? Stickers { get; set; }

    [JsonPropertyName("components")]
    public JsonComponent[]? Components { get; set; }
}
