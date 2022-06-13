using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonThreadUser : JsonThreadSelfUser
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("id")]
    public Snowflake ThreadId { get; init; }
}
