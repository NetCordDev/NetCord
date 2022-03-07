using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildUserChunkEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("members")]
    public JsonGuildUser[] Users { get; init; }

    [JsonPropertyName("chunk_index")]
    public int ChunkIndex { get; init; }

    [JsonPropertyName("chunk_count")]
    public int ChunkCount { get; init; }

    [JsonPropertyName("not_found")]
    public DiscordId[]? NotFound { get; init; }

    [JsonPropertyName("presences")]
    public JsonPresence[]? Presences { get; init; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; init; }
}