using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonCollectibles
{
    [JsonPropertyName("nameplate")]
    public JsonNameplate? Nameplate { get; set; }
}
