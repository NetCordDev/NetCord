using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildUserChunkEventArgs
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("members")]
    public required JsonGuildUser[] Users { get; set; }

    [JsonPropertyName("chunk_index")]
    public required int ChunkIndex { get; set; }

    [JsonPropertyName("chunk_count")]
    public required int ChunkCount { get; set; }

    [JsonPropertyName("not_found")]
    public ulong[]? NotFound { get; set; }

    [JsonPropertyName("presences")]
    public JsonPresence[]? Presences { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
