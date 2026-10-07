using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildUserChunkEventArgs(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonModel, RestClient client)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public IReadOnlyList<GuildUser> Users { get; } = CreateUsers(jsonModel, client);

    private static IReadOnlyList<GuildUser> CreateUsers(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonModel, RestClient client)
    {
        var guildId = jsonModel.GuildId;

        return [.. jsonModel.Users.Select(u => new GuildUser(u, guildId, client))];
    }

    public int ChunkIndex { get; } = jsonModel.ChunkIndex;

    public int ChunkCount { get; } = jsonModel.ChunkCount;

    public IReadOnlyList<ulong>? NotFound { get; } = jsonModel.NotFound;

    public IReadOnlyList<Presence>? Presences { get; } = CreatePresences(jsonModel, client);

    private static IReadOnlyList<Presence>? CreatePresences(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonModel, RestClient client)
    {
        if (jsonModel.Presences is not { } presences)
            return null;

        var guildId = jsonModel.GuildId;

        return [.. presences.Select(p => new Presence(p, guildId, client))];
    }

    public string? Nonce { get; } = jsonModel.Nonce;
}
