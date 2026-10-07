using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonGuildTemplateSerializedSourceGuild
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("verification_level")]
    public VerificationLevel? VerificationLevel { get; set; }

    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotificationLevel? DefaultMessageNotificationLevel { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public ContentFilter? ExplicitContentFilterLevel { get; set; }

    [JsonPropertyName("preferred_locale")]
    public string PreferredLocale { get; set; }

    [JsonPropertyName("afk_timeout")]
    public int? AfkTimeout { get; set; }

    [JsonPropertyName("roles")]
    public JsonRole[] Roles { get; set; }

    [JsonPropertyName("channels")]
    public JsonChannel[] Channels { get; set; }

    [JsonPropertyName("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags? SystemChannelFlags { get; set; }

    [JsonPropertyName("icon_hash")]
    public string? IconHash { get; set; }
}

