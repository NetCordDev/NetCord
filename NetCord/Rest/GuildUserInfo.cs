namespace NetCord.Rest;

public class GuildUserInfo : IJsonModel<JsonModels.JsonGuildUserInfo>
{
    JsonModels.JsonGuildUserInfo IJsonModel<JsonModels.JsonGuildUserInfo>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildUserInfo _jsonModel;

    public GuildUserInfo(JsonModels.JsonGuildUserInfo jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(_jsonModel.User, guildId, client);
    }

    public GuildUser User { get; }

    public string? SourceInviteCode => _jsonModel.SourceInviteCode;

    public GuildUserJoinSourceType JoinSourceType => _jsonModel.JoinSourceType;

    public ulong? InviterId => _jsonModel.InviterId;
}
