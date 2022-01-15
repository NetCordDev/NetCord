using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildVanityInvite
{
    [JsonPropertyName("code")]
    public string Code { get; init; }

    [JsonPropertyName("uses")]
    public int Uses { get; init; }
}