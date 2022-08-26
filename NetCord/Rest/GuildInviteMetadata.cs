namespace NetCord.Rest;

public class GuildInviteMetadata : IJsonModel<JsonModels.JsonGuildInviteMetadata>
{
    JsonModels.JsonGuildInviteMetadata IJsonModel<JsonModels.JsonGuildInviteMetadata>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildInviteMetadata _jsonModel;

    public int Uses => _jsonModel.Uses;

    public int MaxUses => _jsonModel.MaxUses;

    public int MaxAge => _jsonModel.MaxAge;

    public bool Temporary => _jsonModel.Temporary;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public GuildInviteMetadata(JsonModels.JsonGuildInviteMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
