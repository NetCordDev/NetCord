using System.Text.Json.Serialization;

namespace NetCord.JsonModels;
internal record JsonUserActivityTimestamps
{
    [JsonConverter(typeof(JsonConverters.MillisecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("start")]
    public DateTimeOffset? StartTime { get; init; }

    [JsonConverter(typeof(JsonConverters.MillisecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("end")]
    public DateTimeOffset? EndTime { get; init; }
}
