using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a guild members chunk event.
/// </summary>
public class GuildUserChunkEventArgs(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonModel, RestClient client) 
    : IJsonModel<JsonModels.EventArgs.JsonGuildUserChunkEventArgs>
{
    JsonModels.EventArgs.JsonGuildUserChunkEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildUserChunkEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the guild.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// A list of guild users returned in this chunk.
    /// </summary>
    public IReadOnlyList<GuildUser> Users { get; } = jsonModel.Users.Select(u => new GuildUser(u, jsonModel.GuildId, client)).ToArray();

    /// <summary>
    /// The chunk index for the current response.
    /// </summary>
    public int ChunkIndex => jsonModel.ChunkIndex;

    /// <summary>
    /// The total number of expected chunks for the request.
    /// </summary>
    public int ChunkCount => jsonModel.ChunkCount;

    /// <summary>
    /// A list of user IDs that were requested but not found in the guild, if applicable.
    /// </summary>
    public IReadOnlyList<ulong>? NotFound => jsonModel.NotFound;

    /// <summary>
    /// A list of presence objects for the users in this chunk, if requested.
    /// </summary>
    public IReadOnlyList<Presence>? Presences { get; } = jsonModel.Presences is { } presences 
        ? presences.Select(p => new Presence(p, jsonModel.GuildId, client)).ToArray() 
        : null;

    /// <summary>
    /// The developer-defined string that was passed to the request, used to identify the chunk transaction.
    /// </summary>
    public string? Nonce => jsonModel.Nonce;
}
