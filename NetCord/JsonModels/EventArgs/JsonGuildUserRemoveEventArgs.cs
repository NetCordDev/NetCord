using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonGuildUserRemoveEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }
}
