using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildIntegrationDeleteEventArgs
{
    [JsonPropertyName("id")]
    public Snowflake IntegrationId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; set; }

    [JsonSerializable(typeof(JsonGuildIntegrationDeleteEventArgs))]
    public partial class JsonGuildIntegrationDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildIntegrationDeleteEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
