using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public class GuildUserProperties
    {
        [JsonPropertyName("nick")]
        public string? Nickname { get; set; }
        [JsonPropertyName("roles")]
        public IEnumerable<DiscordId>? NewRolesIds { get; set; }
        [JsonPropertyName("mute")]
        public bool? Muted { get; set; }
        [JsonPropertyName("deaf")]
        public bool? Deafened { get; set; }
        [JsonPropertyName("channel_id")]
        public DiscordId? ChannelId { get; set; }
    }
}
