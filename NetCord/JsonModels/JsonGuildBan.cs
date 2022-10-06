using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildBan
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonSerializable(typeof(JsonGuildBan))]
    public partial class JsonGuildBanSerializerContext : JsonSerializerContext
    {
        public static JsonGuildBanSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonGuildBan[]))]
    public partial class JsonGuildBanArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildBanArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
