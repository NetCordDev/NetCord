using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal partial class JsonGuildPruneResult
{
    [JsonPropertyName("pruned")]
    public int? Pruned { get; set; }

    [JsonSerializable(typeof(JsonGuildPruneResult))]
    public partial class JsonGuildPruneResultSerializerContext : JsonSerializerContext
    {
        public static JsonGuildPruneResultSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
