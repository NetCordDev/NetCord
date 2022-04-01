using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonThreadUser
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("id")]
    public Snowflake ThreadId { get; init; }

    [JsonPropertyName("join_timestamp")]
    public DateTimeOffset JoinTimestamp { get; init; }

    [JsonPropertyName("flags")]
    public int Flags { get; init; }
}
