using System.Text.Json.Serialization;

namespace NetCord
{
    public class GuildUserProperties
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("nick")]
        public string? Nickname { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("roles")]
        public IEnumerable<DiscordId>? NewRolesIds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("mute")]
        public bool? Muted { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("deaf")]
        public bool? Deafened { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_id")]
        public DiscordId? ChannelId { get; set; }
    }
}
