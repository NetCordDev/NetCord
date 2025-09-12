using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class DaveMlsInvalidCommitWelcomeProperties(int transitionId)
{
    [JsonPropertyName("transition_id")]
    public int TransitionId { get; set; } = transitionId;
}
