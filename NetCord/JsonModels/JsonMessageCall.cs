using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageCall
{
    [JsonPropertyName("participants")]
    public required ulong[] Participants { get; set; }

    [JsonPropertyName("ended_timestamp")]
    public DateTimeOffset? EndedAt { get; set; }
}
