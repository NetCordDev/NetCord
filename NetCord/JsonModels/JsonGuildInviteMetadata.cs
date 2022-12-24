using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildInviteMetadata
{
    [JsonPropertyName("uses")]
    public int Uses { get; set; }

    [JsonPropertyName("max_uses")]
    public int MaxUses { get; set; }

    [JsonPropertyName("max_age")]
    public int MaxAge { get; set; }

    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonSerializable(typeof(JsonGuildInviteMetadata))]
    public partial class JsonGuildInviteMetadataSerializerContext : JsonSerializerContext
    {
        public static JsonGuildInviteMetadataSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
