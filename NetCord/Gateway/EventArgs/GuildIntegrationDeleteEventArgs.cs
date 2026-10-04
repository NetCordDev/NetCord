using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildIntegrationDeleteEventArgs(JsonGuildIntegrationDeleteEventArgs jsonModel) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;
}
