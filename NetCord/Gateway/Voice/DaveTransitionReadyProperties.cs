using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class DaveTransitionReadyProperties(ushort transitionId)
{
    [JsonPropertyName("transition_id")]
    public ushort TransitionId { get; set; } = transitionId;
}
