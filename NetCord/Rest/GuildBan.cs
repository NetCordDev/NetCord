namespace NetCord.Rest;

public class GuildBan : IJsonModel<JsonModels.JsonGuildBan>
{
    JsonModels.JsonGuildBan IJsonModel<JsonModels.JsonGuildBan>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildBan _jsonModel;

    public string? Reason => _jsonModel.Reason;

    public User User { get; }

    public GuildBan(JsonModels.JsonGuildBan jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client);
    }
}