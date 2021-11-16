using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonUserActivityButton
{
    [JsonPropertyName("label")]
    public string Label { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; }
}
