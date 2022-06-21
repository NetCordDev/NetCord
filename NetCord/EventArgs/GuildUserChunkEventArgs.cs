using NetCord.Rest;

namespace NetCord;

public class GuildUserChunkEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildUserChunkEventArgs>
{
    JsonModels.EventArgs.JsonGuildUserChunkEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildUserChunkEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildUserChunkEventArgs _jsonModel;

    public GuildUserChunkEventArgs(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Users = jsonModel.Users.ToDictionary(u => u.User.Id, u => new GuildUser(u, jsonModel.GuildId, client));
        if (jsonModel.Presences != null)
            Presences = jsonModel.Presences.ToDictionary(p => p.User.Id, p => new Presence(p, client));
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public IReadOnlyDictionary<Snowflake, GuildUser> Users { get; }

    public int ChunkIndex => _jsonModel.ChunkIndex;

    public int ChunkCount => _jsonModel.ChunkCount;

    public IEnumerable<Snowflake>? NotFound => _jsonModel.NotFound;

    public IReadOnlyDictionary<Snowflake, Presence>? Presences { get; }

    public string? Nonce => _jsonModel.Nonce;
}