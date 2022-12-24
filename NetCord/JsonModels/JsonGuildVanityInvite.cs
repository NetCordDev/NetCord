using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildVanityInvite
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }

    [JsonSerializable(typeof(JsonGuildVanityInvite))]
    public partial class JsonGuildVanityInviteSerializerContext : JsonSerializerContext
    {
        public static JsonGuildVanityInviteSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
