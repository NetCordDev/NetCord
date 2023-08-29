using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageReactionCountDetails
{
    [JsonPropertyName("burst")]
    public int Burst { get; set; }

    [JsonPropertyName("normal")]
    public int Normal { get; set; }
}
