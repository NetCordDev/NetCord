using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntity
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
}
