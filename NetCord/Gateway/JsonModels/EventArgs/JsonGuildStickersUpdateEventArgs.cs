using System.Collections.Immutable;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public partial class JsonGuildStickersUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("stickers")]
    public ImmutableArray<JsonSticker> Stickers { get; set; }

    [JsonSerializable(typeof(JsonGuildStickersUpdateEventArgs))]
    public partial class JsonGuildStickersUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildStickersUpdateEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
