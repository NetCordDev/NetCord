namespace NetCord.Rest;

public class GuildVanityInvite(JsonModels.JsonGuildVanityInvite jsonModel) : IJsonModel<JsonModels.JsonGuildVanityInvite>
{
    JsonModels.JsonGuildVanityInvite IJsonModel<JsonModels.JsonGuildVanityInvite>.JsonModel => jsonModel;

    public string Code => jsonModel.Code;

    public int Uses => jsonModel.Uses;
}
