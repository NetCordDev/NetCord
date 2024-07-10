using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePollAnswerCount
{
    [JsonPropertyName("id")]
    public int AnswerId { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}
