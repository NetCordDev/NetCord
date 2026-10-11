using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildIntegrationsUpdateEventArgs(JsonGuildIntegrationsUpdateEventArgs jsonModel)
{
    public ulong GuildId { get; } = jsonModel.GuildId;
}
