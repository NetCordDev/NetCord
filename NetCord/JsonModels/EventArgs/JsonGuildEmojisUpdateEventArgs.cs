using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildEmojisUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("emojis")]
    public JsonEmoji[] Emojis { get; init; }
}