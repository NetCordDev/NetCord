namespace NetCord.Rest;

public class GuildInviteMetadata(JsonModels.JsonRestGuildInviteMetadata jsonModel) : IJsonModel<JsonModels.JsonRestGuildInviteMetadata>
{
    JsonModels.JsonRestGuildInviteMetadata IJsonModel<JsonModels.JsonRestGuildInviteMetadata>.JsonModel => jsonModel;

    public int Uses => jsonModel.Uses;

    public int MaxUses => jsonModel.MaxUses;

    public int MaxAge => jsonModel.MaxAge;

    public bool Temporary => jsonModel.Temporary;

    public DateTimeOffset CreatedAt => jsonModel.CreatedAt;
}
