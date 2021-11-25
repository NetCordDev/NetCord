using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonParty : JsonEntity
{
    [JsonPropertyName("size")]
    public JsonPartySize? Size { get; init; }
}

internal record JsonPartySize
{
    [JsonPropertyName("current_size")]
    public int CurrentSize { get; init; }

    [JsonPropertyName("max_size")]
    public int MaxSize { get; init; }
}
