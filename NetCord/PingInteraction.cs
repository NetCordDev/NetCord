using NetCord.Rest;

namespace NetCord;

public class PingInteraction : Entity, IInteraction
{
    JsonModels.JsonInteraction IJsonModel<JsonModels.JsonInteraction>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteraction _jsonModel;

    private readonly InteractionResponseDelegate _sendResponseAsync;

    public PingInteraction(JsonModels.JsonInteraction jsonModel, InteractionResponseDelegate sendResponseAsync, RestClient client)
    {
        _jsonModel = jsonModel;

        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);

        Entitlements = jsonModel.Entitlements.Select(e => new Entitlement(e, client)).ToArray();

        _sendResponseAsync = sendResponseAsync;
    }

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public User User { get; }

    public string Token => _jsonModel.Token;

    public int Version => _jsonModel.Version;

    public Permissions AppPermissions => _jsonModel.AppPermissions;

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public long AttachmentSizeLimit => _jsonModel.AttachmentSizeLimit;

    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => _sendResponseAsync(this, callback, withResponse, properties, cancellationToken);
}
