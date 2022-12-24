using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal partial class JsonGuildPruneCountResult
{
    [JsonPropertyName("pruned")]
    public int Pruned { get; set; }

    [JsonSerializable(typeof(JsonGuildPruneCountResult))]
    public partial class JsonGuildPruneCountResultSerializerContext : JsonSerializerContext
    {
        public static JsonGuildPruneCountResultSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
