using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class ReadyEventArgs(JsonReadyEventArgs jsonModel, RestClient client)
{
    public ApiVersion Version { get; } = jsonModel.Version;

    public CurrentUser User { get; } = new(jsonModel.User, client);

    public IReadOnlyList<ulong> GuildIds { get; } = [.. jsonModel.Guilds.Select(g => g.Id)];

    public string SessionId { get; } = jsonModel.SessionId;

    public string ResumeGatewayUrl { get; } = jsonModel.ResumeGatewayUrl;

    public Shard? Shard { get; } = jsonModel.Shard;

    public ulong ApplicationId { get; } = jsonModel.Application.Id;

    public ApplicationFlags? ApplicationFlags { get; } = jsonModel.Application.Flags;
}
