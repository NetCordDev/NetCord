using System.Text.Json.Serialization;

namespace NetCord
{
    public class SocketInteraction<T> : ISocketInteraction where T : SocketButtonInteractionData
    {
        [JsonInclude]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("id")]
        public DiscordId Id { get; private init; }
        [JsonInclude]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("application_id")]
        public DiscordId ApplicationId { get; private init; }
        [JsonInclude]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("guild_id")]
        public DiscordId GuildId { get; private init; }
        [JsonInclude]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("channel_id")]
        public DiscordId ChannelId { get; private init; }
        [JsonInclude]
        [JsonPropertyName("user")]
        public User User { get; private init; }
        [JsonInclude]
        [JsonPropertyName("token")]
        public string Token { get; private init; }
        [JsonInclude]
        [JsonPropertyName("message")]
        public Message Message { get; private init; }
        [JsonInclude]
        [JsonPropertyName("data")]
        public T Data { get; private init; }
    }
}