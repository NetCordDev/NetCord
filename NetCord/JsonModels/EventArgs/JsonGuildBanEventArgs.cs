using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildBanEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonSerializable(typeof(JsonGuildBanEventArgs))]
    public partial class JsonGuildBanEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildBanEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
