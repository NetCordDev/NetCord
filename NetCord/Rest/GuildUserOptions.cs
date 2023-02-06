using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildUserOptions : CurrentGuildUserOptions
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    public IEnumerable<ulong>? RoleIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mute")]
    public bool? Muted { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deaf")]
    public bool? Deafened { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonConverter(typeof(JsonConverters.NullableDateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("communication_disabled_until")]
    public DateTimeOffset? TimeOutUntil { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public GuildUserFlags? GuildFlags { get; set; }

    [JsonSerializable(typeof(GuildUserOptions))]
    public partial class GuildUserOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildUserOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
