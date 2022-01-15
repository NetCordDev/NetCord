namespace NetCord;

public class GuildVanityInvite
{
    private readonly JsonModels.JsonGuildVanityInvite _jsonEntity;

    public string Code => _jsonEntity.Code;

    public int Uses => _jsonEntity.Uses;

    internal GuildVanityInvite(JsonModels.JsonGuildVanityInvite jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}