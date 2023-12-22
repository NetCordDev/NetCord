using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonGuildPruneCountResult
{
    [JsonPropertyName("pruned")]
    public int Pruned { get; set; }
}
