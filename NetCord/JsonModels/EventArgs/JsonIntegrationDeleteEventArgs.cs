using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonIntegrationDeleteEventArgs
{
    [JsonPropertyName("id")]
    public Snowflake IntegrationId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; set; }

    [JsonSerializable(typeof(JsonIntegrationDeleteEventArgs))]
    public partial class JsonIntegrationDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonIntegrationDeleteEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
