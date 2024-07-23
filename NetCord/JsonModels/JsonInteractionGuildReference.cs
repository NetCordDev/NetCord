using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonInteractionGuildReference : JsonEntity
{
    [JsonPropertyName("features")]
    public string[] Features { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; }
}
