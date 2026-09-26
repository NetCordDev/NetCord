using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal class JsonDaveExecuteTransition
{
    [JsonPropertyName("transition_id")]
    public ushort TransitionId { get; set; }
}
