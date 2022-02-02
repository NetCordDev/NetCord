using System.Text.Json.Serialization;

namespace NetCord
{
    public class GuildRolePosition
    {
        [JsonPropertyName("id")]
        public DiscordId Id { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("position")]
        public int? Position { get; set; }

        public GuildRolePosition(DiscordId id)
        {
            Id = id;
        }
    }
}