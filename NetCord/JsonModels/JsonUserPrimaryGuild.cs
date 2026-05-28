using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonUserPrimaryGuild
{
    [JsonPropertyName("identity_guild_id")]
    public ulong? IdentityGuildId { get; set; }

    [JsonPropertyName("identity_enabled")]
    public bool? IdentityEnabled { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    [JsonPropertyName("badge")]
    public string? BadgeHash { get; set; }
}
