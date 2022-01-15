namespace NetCord;

public class GuildInviteMetadata
{
    private readonly JsonModels.JsonGuildInviteMetadata _jsonEntity;

    public int Uses => _jsonEntity.Uses;

    public int MaxUses => _jsonEntity.MaxUses;

    public int MaxAge => _jsonEntity.MaxAge;

    public bool Temporary => _jsonEntity.Temporary;

    public DateTimeOffset CreatedAt => _jsonEntity.CreatedAt;

    internal GuildInviteMetadata(JsonModels.JsonGuildInviteMetadata jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}