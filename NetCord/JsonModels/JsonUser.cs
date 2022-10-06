using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonUser : JsonEntity
{
    [JsonPropertyName("username")]
    public virtual string Username { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("discriminator")]
    public virtual ushort Discriminator { get; set; }

    [JsonPropertyName("avatar")]
    public virtual string? AvatarHash { get; set; }

    [JsonPropertyName("bot")]
    public virtual bool IsBot { get; set; }

    [JsonPropertyName("system")]
    public virtual bool? IsSystemUser { get; set; }

    [JsonPropertyName("mfa_enabled")]
    public virtual bool? MfaEnabled { get; set; }

    [JsonPropertyName("banner")]
    public string? BannerHash { get; set; }

    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; set; }

    [JsonPropertyName("locale")]
    public virtual CultureInfo? Locale { get; set; }

    [JsonPropertyName("verified")]
    public virtual bool? Verified { get; set; }

    [JsonPropertyName("email")]
    public virtual string? Email { get; set; }

    [JsonPropertyName("flags")]
    public virtual UserFlags? Flags { get; set; }

    [JsonPropertyName("premium_type")]
    public virtual PremiumType? PremiumType { get; set; }

    [JsonPropertyName("public_flags")]
    public virtual UserFlags? PublicFlags { get; set; }

    [JsonSerializable(typeof(JsonUser))]
    public partial class JsonUserSerializerContext : JsonSerializerContext
    {
        public static JsonUserSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonUser[]))]
    public partial class JsonUserArraySerializerContext : JsonSerializerContext
    {
        public static JsonUserArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
