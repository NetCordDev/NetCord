using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class DMChannelProperties(ulong userId)
{
    /// <summary>
    /// The recipient to open a DM channel with.
    /// </summary>
    [JsonPropertyName("recipient_id")]
    public ulong UserId { get; set; } = userId;
}
