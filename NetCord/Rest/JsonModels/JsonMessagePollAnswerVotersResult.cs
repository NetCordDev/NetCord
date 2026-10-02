using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonMessagePollAnswerVotersResult
{
    [JsonPropertyName("users")]
    public required JsonUser[] Users { get; set; }
}
