using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonMessage : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("author")]
    public JsonUser Author { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? Member { get; init; }

    [JsonPropertyName("content")]
    public string Content { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedAt { get; init; }

    [JsonPropertyName("tts")]
    public bool IsTts { get; init; }

    [JsonPropertyName("mention_everyone")]
    public bool MentionEveryone { get; init; }

    [JsonPropertyName("mentions")]
    public JsonUser[] MentionedUsers { get; init; }

    [JsonPropertyName("mention_roles")]
    public Snowflake[] MentionedRoleIds { get; init; }

    [JsonPropertyName("mention_channels")]
    public JsonGuildChannelMention[]? MentionedChannels { get; init; }

    [JsonPropertyName("attachments")]
    public JsonAttachment[] Attachments { get; init; }

    [JsonPropertyName("embeds")]
    public JsonEmbed[] Embeds { get; init; }

    [JsonPropertyName("reactions")]
    public JsonMessageReaction[]? Reactions { get; init; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; init; }

    [JsonPropertyName("pinned")]
    public bool IsPinned { get; init; }

    [JsonPropertyName("webhook_id")]
    public Snowflake? WebhookId { get; init; }

    [JsonPropertyName("type")]
    public MessageType Type { get; init; }

    [JsonPropertyName("activity")]
    public JsonMessageActivity? Activity { get; init; }

    [JsonPropertyName("application")]
    public JsonApplication? Application { get; init; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; init; }

    [JsonPropertyName("message_reference")]
    public JsonMessageReference? MessageReference { get; init; }

    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; init; }

    [JsonPropertyName("referenced_message")]
    public JsonMessage? ReferencedMessage { get; init; }

    [JsonPropertyName("interaction")]
    public JsonMessageInteraction? Interaction { get; init; }

    [JsonPropertyName("thread")]
    public JsonChannel? StartedThread { get; init; }

    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; init; }

    [JsonPropertyName("sticker_items")]
    public JsonMessageSticker[]? Stickers { get; init; }
}
