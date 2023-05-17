namespace NetCord.Rest;

public class GuildInviteMetadata : IJsonModel<JsonModels.JsonRestGuildInviteMetadata>
{
    JsonModels.JsonRestGuildInviteMetadata IJsonModel<JsonModels.JsonRestGuildInviteMetadata>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonRestGuildInviteMetadata _jsonModel;

    public int Uses => _jsonModel.Uses;

    public int MaxUses => _jsonModel.MaxUses;

    public int MaxAge => _jsonModel.MaxAge;

    public bool Temporary => _jsonModel.Temporary;

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;

    public GuildInviteMetadata(JsonModels.JsonRestGuildInviteMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
