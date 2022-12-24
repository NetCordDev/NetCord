using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildIntegrationsUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonSerializable(typeof(JsonGuildIntegrationsUpdateEventArgs))]
    public partial class JsonGuildIntegrationsUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildIntegrationsUpdateEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
