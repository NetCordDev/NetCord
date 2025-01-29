using NetCord.Rest;

namespace NetCord;

public class PingInteraction : Entity, IInteraction
{
    public PingInteraction(JsonModels.JsonInteraction jsonModel, Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> sendResponseAsync, RestClient client)
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

    JsonModels.JsonInteraction IJsonModel<JsonModels.JsonInteraction>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteraction _jsonModel;

    private readonly Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> _sendResponseAsync;

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public User User { get; }

    public string Token => _jsonModel.Token;

    public Permissions AppPermissions => _jsonModel.AppPermissions;

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public Task SendResponseAsync(InteractionCallback callback, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => _sendResponseAsync(this, callback, properties, cancellationToken);
}
