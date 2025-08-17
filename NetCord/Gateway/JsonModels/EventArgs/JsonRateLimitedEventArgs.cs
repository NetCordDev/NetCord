using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonRateLimitedEventArgs
{
    [JsonPropertyName("opcode")]
    public GatewayOpcode Opcode { get; set; }

    [JsonPropertyName("retry_after")]
    public double RetryAfter { get; set; }

    [JsonPropertyName("meta")]
    public JsonElement Metadata { get; set; }
}

public class JsonRequestGuildUsersRateLimitMetadata
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
