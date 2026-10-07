using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class InviteDeleteEventArgs(JsonInviteDeleteEventArgs jsonModel)
{
    public ulong InviteChannelId { get; } = jsonModel.InviteChannelId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public string InviteCode { get; } = jsonModel.InviteCode;
}
