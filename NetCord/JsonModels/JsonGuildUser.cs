using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildUser
{
    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }

    [JsonPropertyName("avatar")]
    public string? GuildAvatarHash { get; set; }

    [JsonPropertyName("roles")]
    public Snowflake[] RoleIds { get; set; }

    [JsonPropertyName("hoisted_role")]
    public Snowflake? HoistedRoleId { get; set; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset JoinedAt { get; set; }

    [JsonPropertyName("premium_since")]
    public DateTimeOffset? GuildBoostStart { get; set; }

    [JsonPropertyName("deaf")]
    public bool Deafened { get; set; }

    [JsonPropertyName("mute")]
    public bool Muted { get; set; }

    [JsonPropertyName("pending")]
    public bool? IsPending { get; set; }

    [JsonPropertyName("permissions")]
    public Permission? Permissions { get; set; }

    [JsonPropertyName("communication_disabled_until")]
    public DateTimeOffset? TimeOutUntil { get; set; }

    [JsonSerializable(typeof(JsonGuildUser))]
    public partial class JsonGuildUserSerializerContext : JsonSerializerContext
    {
        public static JsonGuildUserSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonGuildUser[]))]
    public partial class JsonGuildUserArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildUserArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
