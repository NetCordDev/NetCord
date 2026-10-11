using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildUserInfo(JsonGuildUserInfo jsonModel, ulong guildId, RestClient client)
{
    /// <summary>
    /// The <see cref="GuildUser"/> object representing the user.
    /// </summary>
    public GuildUser User { get; } = new(jsonModel.User, guildId, client);

    /// <summary>
    /// The code of the invite the <see cref="User"/> joined from.
    /// </summary>
    public string? SourceInviteCode { get; } = jsonModel.SourceInviteCode;

    /// <summary>
    /// Specifies how the <see cref="User"/> joined the guild.
    /// </summary>
    public GuildUserJoinSourceType JoinSourceType { get; } = jsonModel.JoinSourceType;

    /// <summary>
    /// The ID of the user that invited the <see cref="User"/> to the guild.
    /// </summary>
    public ulong? InviterId { get; } = jsonModel.InviterId;
}
