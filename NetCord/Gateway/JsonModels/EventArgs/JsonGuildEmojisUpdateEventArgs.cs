using System.Collections.Immutable;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public partial class JsonGuildEmojisUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("emojis")]
    public ImmutableArray<JsonEmoji> Emojis { get; set; }

    [JsonSerializable(typeof(JsonGuildEmojisUpdateEventArgs))]
    public partial class JsonGuildEmojisUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildEmojisUpdateEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
