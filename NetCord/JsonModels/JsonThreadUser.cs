using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonThreadUser : JsonThreadSelfUser
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("id")]
    public Snowflake ThreadId { get; init; }
}
