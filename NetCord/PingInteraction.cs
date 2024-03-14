using NetCord.Rest;

namespace NetCord;

public class PingInteraction : Entity, IInteraction
{
    JsonModels.JsonInteraction IJsonModel<JsonModels.JsonInteraction>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteraction _jsonModel;

    private readonly Func<IInteraction, InteractionCallback, RestRequestProperties?, Task> _sendResponseAsync;

    public PingInteraction(JsonModels.JsonInteraction jsonModel, Func<IInteraction, InteractionCallback, RestRequestProperties?, Task> sendResponseAsync, RestClient client)
    {
        _jsonModel = jsonModel;

        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);

        Entitlements = jsonModel.Entitlements.Select(e => new Entitlement(e)).ToArray();

        _sendResponseAsync = sendResponseAsync;
    }

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public User User { get; }

    public string Token => _jsonModel.Token;

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public Task SendResponseAsync(InteractionCallback callback, RestRequestProperties? properties = null) => _sendResponseAsync(this, callback, properties);
}
