namespace NetCord.Rest;

public class GuildVanityInvite : IJsonModel<JsonModels.JsonGuildVanityInvite>
{
    JsonModels.JsonGuildVanityInvite IJsonModel<JsonModels.JsonGuildVanityInvite>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildVanityInvite _jsonModel;

    public string Code => _jsonModel.Code;

    public int Uses => _jsonModel.Uses;

    public GuildVanityInvite(JsonModels.JsonGuildVanityInvite jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
