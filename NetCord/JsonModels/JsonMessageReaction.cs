using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageReaction
{
    [JsonPropertyName("count")]
    public required int Count { get; set; }

    [JsonPropertyName("count_details")]
    public required JsonMessageReactionCountDetails CountDetails { get; set; }

    [JsonPropertyName("me")]
    public required bool Me { get; set; }

    [JsonPropertyName("me_burst")]
    public required bool MeBurst { get; set; }

    [JsonPropertyName("emoji")]
    public required JsonEmoji Emoji { get; set; }

    [JsonPropertyName("burst_colors")]
    public required Color[] BurstColors { get; set; }
}
