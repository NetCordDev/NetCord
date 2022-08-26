using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonThreadSelfUser
{
    [JsonPropertyName("join_timestamp")]
    public DateTimeOffset JoinTimestamp { get; init; }

    [JsonPropertyName("flags")]
    public int Flags { get; init; }
}
