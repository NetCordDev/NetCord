namespace NetCord.Rest;

public class GuildVanityInvite(JsonModels.JsonGuildVanityInvite jsonModel)
{
    public string Code { get; } = jsonModel.Code;

    public int Uses { get; } = jsonModel.Uses;
}
