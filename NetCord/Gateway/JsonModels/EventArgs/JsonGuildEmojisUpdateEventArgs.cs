using System.Collections.Immutable;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildEmojisUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("emojis")]
    public ImmutableArray<JsonEmoji> Emojis { get; set; }
}
