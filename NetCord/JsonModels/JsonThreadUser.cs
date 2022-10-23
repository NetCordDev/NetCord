using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonThreadUser : JsonThreadSelfUser
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("id")]
    public ulong ThreadId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonPropertyName("presence")]
    public JsonPresence? Presence { get; set; }

    [JsonSerializable(typeof(JsonThreadUser))]
    public partial class JsonThreadUserSerializerContext : JsonSerializerContext
    {
        public static JsonThreadUserSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonThreadUser[]))]
    public partial class JsonThreadUserArraySerializerContext : JsonSerializerContext
    {
        public static JsonThreadUserArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
