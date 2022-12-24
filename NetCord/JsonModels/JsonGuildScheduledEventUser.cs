using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildScheduledEventUser
{
    [JsonPropertyName("")]
    public ulong ScheduledEventId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonSerializable(typeof(JsonGuildScheduledEventUser))]
    public partial class JsonGuildScheduledEventUserSerializerContext : JsonSerializerContext
    {
        public static JsonGuildScheduledEventUserSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonGuildScheduledEventUser[]))]
    public partial class JsonGuildScheduledEventUserArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildScheduledEventUserArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
