using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonVoiceServerUpdateEventArgs
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
}
