using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;
public partial class JsonUserActivityTimestamps
{
    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("start")]
    public DateTimeOffset? StartTime { get; set; }

    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("end")]
    public DateTimeOffset? EndTime { get; set; }

    [JsonSerializable(typeof(JsonUserActivityTimestamps))]
    public partial class JsonUserActivityTimestampsSerializerContext : JsonSerializerContext
    {
        public static JsonUserActivityTimestampsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
