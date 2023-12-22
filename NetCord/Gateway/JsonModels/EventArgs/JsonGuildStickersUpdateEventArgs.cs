using System.Collections.Immutable;
using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildStickersUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("stickers")]
    public ImmutableArray<JsonSticker> Stickers { get; set; }
}
