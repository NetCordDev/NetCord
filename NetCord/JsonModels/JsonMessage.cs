using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonMessage : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("author")]
    public JsonUser Author { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    //[JsonPropertyName("timestamp")]
    //public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedAt { get; set; }

    [JsonPropertyName("tts")]
    public bool IsTts { get; set; }

    [JsonPropertyName("mention_everyone")]
    public bool MentionsEveryone { get; set; }

    [JsonPropertyName("mentions")]
    public JsonUser[] MentionedUsers { get; set; }

    [JsonPropertyName("mention_roles")]
    public ulong[] MentionedRoleIds { get; set; }

    [JsonPropertyName("mention_channels")]
    public JsonGuildChannelMention[]? MentionedChannels { get; set; }

    [JsonPropertyName("attachments")]
    public JsonAttachment[] Attachments { get; set; }

    [JsonPropertyName("embeds")]
    public JsonEmbed[] Embeds { get; set; }

    [JsonPropertyName("reactions")]
    public JsonMessageReaction[]? Reactions { get; set; }

    [JsonConverter(typeof(JsonConverters.AnyValueToStringConverter))]
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
    public JsonPartialApplication? Application { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonPropertyName("message_reference")]
    public JsonMessageReference? MessageReference { get; set; }

    [JsonPropertyName("message_snapshots")]
    public JsonMessageSnapshot[]? MessageSnapshots { get; set; }

    [JsonPropertyName("referenced_message")]
    public JsonMessage? ReferencedMessage { get; set; }

    [JsonPropertyName("interaction_metadata")]
    public JsonMessageInteractionMetadata? InteractionMetadata { get; set; }

    [Obsolete($"Replaced by '{nameof(InteractionMetadata)}'")]
    [JsonPropertyName("interaction")]
    public JsonMessageInteraction? Interaction { get; set; }

    [JsonPropertyName("thread")]
    public JsonChannel? StartedThread { get; set; }

    [JsonPropertyName("components")]
    public JsonComponent[]? Components { get; set; }

    [JsonPropertyName("sticker_items")]
    public JsonMessageSticker[]? Stickers { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("role_subscription_data")]
    public JsonRoleSubscriptionData? RoleSubscriptionData { get; set; }

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }

    [JsonPropertyName("poll")]
    public JsonMessagePoll? Poll { get; set; }

    [JsonPropertyName("call")]
    public JsonMessageCall? Call { get; set; }

    [JsonPropertyName("shared_client_theme")]
    public JsonSharedClientTheme? SharedClientTheme { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonPropertyName("channel_type")]
    public ChannelType? ChannelType { get; set; }
}
