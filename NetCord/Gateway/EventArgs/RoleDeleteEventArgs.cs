using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class RoleDeleteEventArgs(JsonRoleDeleteEventArgs jsonModel)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong RoleId { get; } = jsonModel.RoleId;
}
