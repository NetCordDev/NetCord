using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonGuildPruneResult
{
    [JsonPropertyName("pruned")]
    public int? Pruned { get; set; }
}
