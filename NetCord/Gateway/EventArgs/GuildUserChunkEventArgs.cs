using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildUserChunkEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildUserChunkEventArgs>
{
    JsonModels.EventArgs.JsonGuildUserChunkEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildUserChunkEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildUserChunkEventArgs _jsonModel;

    public GuildUserChunkEventArgs(JsonModels.EventArgs.JsonGuildUserChunkEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Users = jsonModel.Users.Select(u => new GuildUser(u, jsonModel.GuildId, client)).ToArray();

        var presences = jsonModel.Presences;
        if (presences is not null)
            Presences = presences.Select(p => new Presence(p, jsonModel.GuildId, client)).ToArray();
    }

    public ulong GuildId => _jsonModel.GuildId;

    public IReadOnlyList<GuildUser> Users { get; }

    public int ChunkIndex => _jsonModel.ChunkIndex;

    public int ChunkCount => _jsonModel.ChunkCount;

    public IReadOnlyList<ulong>? NotFound => _jsonModel.NotFound;

    public IReadOnlyList<Presence>? Presences { get; }

    public string? Nonce => _jsonModel.Nonce;
}
