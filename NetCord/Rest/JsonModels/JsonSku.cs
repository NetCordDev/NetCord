using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public partial class JsonSku : JsonEntity
{
    [JsonPropertyName("type")]
    public SkuType Type { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("flags")]
    public SkuFlags Flags { get; set; }

    [JsonSerializable(typeof(JsonSku[]))]
    public partial class JsonSkuArraySerializerContext : JsonSerializerContext
    {
        public static JsonSkuArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
