using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonMessageCall
{
    [JsonPropertyName("participants")]
    public ulong[] Participants { get; set; }

    [JsonPropertyName("ended_timestamp")]
    public DateTimeOffset? EndedAt { get; set; }
}
