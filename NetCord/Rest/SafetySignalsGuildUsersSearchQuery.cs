using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class SafetySignalsGuildUsersSearchQuery : IGuildUsersSearchQuery
{
    private static readonly JsonEncodedText _safetySignals = JsonEncodedText.Encode("safety_signals");

    [JsonConverter(typeof(DateTimeOffsetToGreaterThanOrEqualToRangeConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("unusual_dm_activity_until")]
    public DateTimeOffset? UnusualDMActivityUntil { get; set; }

    [JsonConverter(typeof(DateTimeOffsetToGreaterThanOrEqualToRangeConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("communication_disabled_until")]
    public DateTimeOffset? TimedOutUntil { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("unusual_account_activity")]
    public bool? UnusualAccountActivity { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("automod_quarantined_username")]
    public bool? AutoModerationQuarantinedUsername { get; set; }

    public void Serialize(Utf8JsonWriter writer)
    {
        writer.WritePropertyName(_safetySignals);
        JsonSerializer.Serialize(writer, this, Serialization.Default.SafetySignalsGuildUsersSearchQuery);
    }

    public class DateTimeOffsetToGreaterThanOrEqualToRangeConverter : JsonConverter<DateTimeOffset>
    {
        private static readonly JsonEncodedText _range = JsonEncodedText.Encode("range");
        private static readonly JsonEncodedText _gte = JsonEncodedText.Encode("gte");

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteStartObject(_range);

            writer.WriteNumber(_gte, value.ToUnixTimeMilliseconds());

            writer.WriteEndObject();

            writer.WriteEndObject();
        }
    }
}
