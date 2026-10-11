using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonMessagePollAnswerVotersResult
{
    [JsonPropertyName("users")]
    public JsonUser[] Users { get; set; }
}
