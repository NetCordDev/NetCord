using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessage : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("author")]
    public JsonUser Author { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedAt { get; set; }

    [JsonPropertyName("tts")]
    public bool IsTts { get; set; }

    [JsonPropertyName("mention_everyone")]
    public bool MentionEveryone { get; set; }

    [JsonPropertyName("mentions")]
    public JsonUser[]? MentionedUsers { get; set; }

    [JsonPropertyName("mention_roles")]
    public ulong[]? MentionedRoleIds { get; set; }

    [JsonPropertyName("mention_channels")]
    public JsonGuildChannelMention[]? MentionedChannels { get; set; }

    [JsonPropertyName("attachments")]
    public JsonAttachment[] Attachments { get; set; }

    [JsonPropertyName("embeds")]
    public JsonEmbed[] Embeds { get; set; }

    [JsonPropertyName("reactions")]
    public JsonMessageReaction[]? Reactions { get; set; }

    [JsonConverter(typeof(JsonConverters.NonceConverter))]
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    [JsonPropertyName("pinned")]
    public bool IsPinned { get; set; }

    [JsonPropertyName("webhook_id")]
    public ulong? WebhookId { get; set; }

    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    [JsonPropertyName("activity")]
    public JsonMessageActivity? Activity { get; set; }

    [JsonPropertyName("application")]
    public JsonApplication? Application { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("message_reference")]
    public JsonMessageReference? MessageReference { get; set; }

    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonPropertyName("referenced_message")]
    public JsonMessage? ReferencedMessage { get; set; }

    [JsonPropertyName("interaction")]
    public JsonMessageInteraction? Interaction { get; set; }

    [JsonPropertyName("thread")]
    public JsonChannel? StartedThread { get; set; }

    [JsonPropertyName("components")]
    public JsonComponent[]? Components { get; set; }

    [JsonPropertyName("sticker_items")]
    public JsonMessageSticker[]? Stickers { get; set; }

    [JsonSerializable(typeof(JsonMessage))]
    public partial class JsonMessageSerializerContext : JsonSerializerContext
    {
        public static JsonMessageSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonMessage[]))]
    public partial class JsonMessageArraySerializerContext : JsonSerializerContext
    {
        public static JsonMessageArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
