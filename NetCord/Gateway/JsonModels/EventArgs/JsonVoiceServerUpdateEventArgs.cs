using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Gateway.JsonModels.EventArgs;

[JsonGuard]
public partial class JsonVoiceServerUpdateEventArgs
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
}
