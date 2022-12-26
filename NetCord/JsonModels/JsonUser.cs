using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonUser : JsonEntity
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("discriminator")]
    public ushort Discriminator { get; set; }

    [JsonPropertyName("avatar")]
    public string? AvatarHash { get; set; }

    [JsonPropertyName("bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("system")]
    public bool? IsSystemUser { get; set; }

    [JsonPropertyName("mfa_enabled")]
    public bool? MfaEnabled { get; set; }

    [JsonPropertyName("banner")]
    public string? BannerHash { get; set; }

    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; set; }

    [JsonPropertyName("locale")]
    public CultureInfo? Locale { get; set; }

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

    [JsonPropertyName("avatar_decoration")]
    public string? AvatarDecorationHash { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonSerializable(typeof(JsonUser))]
    public partial class JsonUserSerializerContext : JsonSerializerContext
    {
        public static JsonUserSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonUser[]))]
    public partial class JsonUserArraySerializerContext : JsonSerializerContext
    {
        public static JsonUserArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
