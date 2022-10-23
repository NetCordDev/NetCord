using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildScheduledEventUserEventArgs
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong GuildScheduledEventId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonSerializable(typeof(JsonGuildScheduledEventUserEventArgs))]
    public partial class JsonGuildScheduledEventUserEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildScheduledEventUserEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
