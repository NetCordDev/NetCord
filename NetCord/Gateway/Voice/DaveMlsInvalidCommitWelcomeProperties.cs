using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class DaveMlsInvalidCommitWelcomeProperties(ushort transitionId)
{
    [JsonPropertyName("transition_id")]
    public ushort TransitionId { get; set; } = transitionId;
}
