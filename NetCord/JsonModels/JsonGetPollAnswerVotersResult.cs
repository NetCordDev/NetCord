using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal class JsonGetPollAnswerVotersResult
{
    [JsonPropertyName("users")]
    public JsonUser[] Users { get; set; }
}
