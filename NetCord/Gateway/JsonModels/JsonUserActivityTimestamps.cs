using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

public class JsonUserActivityTimestamps
{
    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("start")]
    public DateTimeOffset? StartTime { get; set; }

    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("end")]
    public DateTimeOffset? EndTime { get; set; }
}
