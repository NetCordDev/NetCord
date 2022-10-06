using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonVoiceRegion
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("optimal")]
    public bool Optimal { get; set; }

    [JsonPropertyName("deprecated")]
    public bool Deprecated { get; set; }

    [JsonPropertyName("custom")]
    public bool Custom { get; set; }

    [JsonSerializable(typeof(JsonVoiceRegion))]
    public partial class JsonVoiceRegionSerializerContext : JsonSerializerContext
    {
        public static JsonVoiceRegionSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonVoiceRegion[]))]
    public partial class JsonVoiceRegionArraySerializerContext : JsonSerializerContext
    {
        public static JsonVoiceRegionArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
