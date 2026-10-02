using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonPresence
{
    [JsonPropertyName("user")]
    public required JsonPresenceUser User { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("status")]
    public UserStatusType? Status { get; set; }

    [JsonPropertyName("activities")]
    public JsonUserActivity[]? Activities { get; set; }

    [JsonPropertyName("client_status")]
    public IReadOnlyDictionary<Platform, UserStatusType>? ClientStatus { get; set; }
}

public class JsonPresenceUser : JsonEntity
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("discriminator")]
    public ushort? Discriminator { get; set; }

    [JsonPropertyName("global_name")]
    public string? GlobalName { get; set; }

    [JsonPropertyName("avatar")]
    public string? AvatarHash { get; set; }

    [JsonPropertyName("bot")]
    public bool? IsBot { get; set; }

    [JsonPropertyName("system")]
    public bool? IsSystemUser { get; set; }

    [JsonPropertyName("mfa_enabled")]
    public bool? MfaEnabled { get; set; }

    [JsonPropertyName("banner")]
    public string? BannerHash { get; set; }

    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("flags")]
    public UserFlags? Flags { get; set; }

    [JsonPropertyName("premium_type")]
    public PremiumType? PremiumType { get; set; }

    [JsonPropertyName("public_flags")]
    public UserFlags? PublicFlags { get; set; }

    [JsonPropertyName("avatar_decoration_data")]
    public JsonAvatarDecorationData? AvatarDecorationData { get; set; }

    [JsonPropertyName("collectibles")]
    public JsonCollectibles? Collectibles { get; set; }

    [JsonPropertyName("primary_guild")]
    public JsonUserPrimaryGuild? PrimaryGuild { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }
}
