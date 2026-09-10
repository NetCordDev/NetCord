using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonThreadCurrentUser
{
    [JsonPropertyName("join_timestamp")]
    public DateTimeOffset JoinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public ThreadUserFlags Flags { get; set; }
}
