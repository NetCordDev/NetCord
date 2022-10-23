using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonIntegrationDeleteEventArgs
{
    [JsonPropertyName("id")]
    public ulong IntegrationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonSerializable(typeof(JsonIntegrationDeleteEventArgs))]
    public partial class JsonIntegrationDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonIntegrationDeleteEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
