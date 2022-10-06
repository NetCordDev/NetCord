using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonRestGuildThreadResult
{
    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; set; }

    [JsonSerializable(typeof(JsonRestGuildThreadResult))]
    public partial class JsonRestGuildThreadResultSerializerContext : JsonSerializerContext
    {
        public static JsonRestGuildThreadResultSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

public partial class JsonRestGuildThreadPartialResult : JsonRestGuildThreadResult
{
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonSerializable(typeof(JsonRestGuildThreadPartialResult))]
    public partial class JsonRestGuildThreadPartialResultSerializerContext : JsonSerializerContext
    {
        public static JsonRestGuildThreadPartialResultSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
