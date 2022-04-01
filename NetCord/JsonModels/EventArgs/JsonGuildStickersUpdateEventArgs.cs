using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildStickersUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("stickers")]
    public JsonSticker[] Stickers { get; init; }
}