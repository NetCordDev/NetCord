using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildStickersUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("stickers")]
    public JsonSticker[] Stickers { get; set; }

    [JsonSerializable(typeof(JsonGuildStickersUpdateEventArgs))]
    public partial class JsonGuildStickersUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildStickersUpdateEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
