namespace NetCord.Rest;

public partial class GuildBan : IJsonModel<JsonModels.JsonGuildBan>
{
    JsonModels.JsonGuildBan IJsonModel<JsonModels.JsonGuildBan>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildBan _jsonModel;

    private readonly RestClient _client;

    public string? Reason => _jsonModel.Reason;

    public User User { get; }

    public ulong GuildId { get; }

    public GuildBan(JsonModels.JsonGuildBan jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client);
        GuildId = guildId;
        _client = client;
    }
}
