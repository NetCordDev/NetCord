using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public partial class JsonWebhook : JsonEntity
{
    [JsonPropertyName("type")]
    public WebhookType Type { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("avatar")]
    public string? AvatarHash { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("source_guild")]
    public JsonGuild? Guild { get; set; }

    [JsonPropertyName("source_channel")]
    public JsonChannel? Channel { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonSerializable(typeof(JsonWebhook))]
    public partial class JsonWebhookSerializerContext : JsonSerializerContext
    {
        public static JsonWebhookSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonWebhook[]))]
    public partial class JsonWebhookArraySerializerContext : JsonSerializerContext
    {
        public static JsonWebhookArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
