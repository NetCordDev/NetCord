using NetCord.Rest;

namespace NetCord.Gateway;

public class ReadyEventArgs : IJsonModel<JsonModels.EventArgs.JsonReadyEventArgs>
{
    JsonModels.EventArgs.JsonReadyEventArgs IJsonModel<JsonModels.EventArgs.JsonReadyEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonReadyEventArgs _jsonModel;

    public ApiVersion Version => _jsonModel.Version;

    public CurrentUser User { get; }

    public IReadOnlyList<ulong> GuildIds { get; }

    public string SessionId => _jsonModel.SessionId;

    public string ResumeGatewayUrl => _jsonModel.ResumeGatewayUrl;

    public Shard? Shard => _jsonModel.Shard;

    public ulong ApplicationId => _jsonModel.Application is null ? default : _jsonModel.Application.Id;

    public ApplicationFlags ApplicationFlags => _jsonModel.Application is null ? default : _jsonModel.Application.Flags.GetValueOrDefault();

    public ReadyEventArgs(JsonModels.EventArgs.JsonReadyEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client);
        GuildIds = _jsonModel.Guilds.Select(g => g.Id).ToArray();
    }
}
