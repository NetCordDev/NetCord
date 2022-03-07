namespace NetCord;

public class GuildUserChunkEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildUserChunkEventArgs _jsonEntity;

    internal GuildUserChunkEventArgs(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Users = jsonEntity.Users.ToDictionary(u => u.User.Id, u => new GuildUser(u, jsonEntity.GuildId, client));
        if (jsonEntity.Presences != null)
            Presences = jsonEntity.Presences.ToDictionary(p => p.User.Id, p => new Presence(p, client));
    }

    public DiscordId GuildId => _jsonEntity.GuildId;

    public IReadOnlyDictionary<DiscordId, GuildUser> Users { get; }

    public int ChunkIndex => _jsonEntity.ChunkIndex;

    public int ChunkCount => _jsonEntity.ChunkCount;

    public IEnumerable<DiscordId>? NotFound => _jsonEntity.NotFound;

    public IReadOnlyDictionary<DiscordId, Presence>? Presences { get; }

    public string? Nonce => _jsonEntity.Nonce;
}