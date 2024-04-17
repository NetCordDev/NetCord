using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessagePoll : MessagePollBase
{
    // Non-expiring posts are possible in the future, see: https://github.com/discord/discord-api-docs/blob/e4bdf50f11f9ca61ace2636285e029a2b3dfd0ec/docs/resources/Poll.md#poll-object
    [JsonPropertyName("expiry")]
    public DateTimeOffset? Expiry { get; set; }
}
