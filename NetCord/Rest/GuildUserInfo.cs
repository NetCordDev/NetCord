using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents information about a guild user, including join source details.
/// </summary>
public class GuildUserInfo(JsonGuildUserInfo jsonModel, ulong guildId, RestClient client) : IJsonModel<JsonGuildUserInfo>
{
    JsonGuildUserInfo IJsonModel<JsonGuildUserInfo>.JsonModel => jsonModel;

    /// <summary>
    /// The <see cref="GuildUser"/> object representing the user.
    /// </summary>
    public GuildUser User { get; } = new(jsonModel.User, guildId, client);

    /// <summary>
    /// The code of the invite the <see cref="User"/> joined from.
    /// </summary>
    public string? SourceInviteCode => jsonModel.SourceInviteCode;

    /// <summary>
    /// Specifies how the <see cref="User"/> joined the guild.
    /// </summary>
    public GuildUserJoinSourceType JoinSourceType => jsonModel.JoinSourceType;

    /// <summary>
    /// The ID of the user that invited the <see cref="User"/> to the guild.
    /// </summary>
    public ulong? InviterId => jsonModel.InviterId;
}
