using System.Text.Json.Serialization;

namespace NetCord.JsonModels
{
    internal record class JsonResolvedData
    {

        [JsonPropertyName("users")]
        public IReadOnlyDictionary<DiscordId, JsonUser> Users { get; init; }

        [JsonPropertyName("members")]
        public IReadOnlyDictionary<DiscordId, JsonGuildUser> GuildUsers { get; init; }

        [JsonPropertyName("roles")]
        public IReadOnlyDictionary<DiscordId, JsonRole> Roles { get; init; }

        [JsonPropertyName("channels")]
        public IReadOnlyDictionary<DiscordId, JsonChannel> Channels { get; init; }

        [JsonPropertyName("channels")]
        public IReadOnlyDictionary<DiscordId, JsonMessage> Messages { get; init; }
    }
}