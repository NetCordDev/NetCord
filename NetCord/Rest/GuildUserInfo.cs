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

    /// <summary>
    /// The <see cref="GuildUser"/> object representing the user.
    /// </summary>
    public GuildUser User { get; }

    /// <summary>
    /// The code of the invite the <see cref="User"/> joined from.
    /// </summary>
    public string? SourceInviteCode => _jsonModel.SourceInviteCode;

    /// <summary>
    /// Specifies how the <see cref="User"/> joined the guild.
    /// </summary>
    public GuildUserJoinSourceType JoinSourceType => _jsonModel.JoinSourceType;

    /// <summary>
    /// The ID of the account that invited the <see cref="User"/> to the guild.
    /// </summary>
    public ulong? InviterId => _jsonModel.InviterId;
}
