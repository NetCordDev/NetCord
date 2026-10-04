using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildIntegrationDeleteEventArgs(JsonGuildIntegrationDeleteEventArgs jsonModel) : Entity(jsonModel)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;
}
