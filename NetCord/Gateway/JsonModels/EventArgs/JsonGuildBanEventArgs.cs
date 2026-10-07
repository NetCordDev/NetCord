using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Gateway.JsonModels.EventArgs;

[JsonGuard]
public partial class JsonGuildBanEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }
}
