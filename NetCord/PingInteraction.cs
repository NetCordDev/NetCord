using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a ping interaction sent by Discord to verify an endpoint's availability.
/// </summary>
public class PingInteraction : Entity, IInteraction
{
    /// <inheritdoc />
    JsonModels.JsonInteraction IJsonModel<JsonModels.JsonInteraction>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteraction _jsonModel;

    private readonly InteractionResponseDelegate _sendResponseAsync;

    public PingInteraction(JsonModels.JsonInteraction jsonModel, InteractionResponseDelegate sendResponseAsync, RestClient client)
    {
        _jsonModel = jsonModel;

        if (jsonModel.GuildId is { } guildId)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId, client);
        else
            User = new User(jsonModel.User!, client);

        Entitlements = jsonModel.Entitlements.Length == 0 
            ? Array.Empty<Entitlement>() 
            : jsonModel.Entitlements.Select(e => new Entitlement(e, client)).ToArray();

        _sendResponseAsync = sendResponseAsync;
    }

    /// <inheritdoc />
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The ID of the application that the interaction belongs to.
    /// </summary>
    public ulong ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// The user who triggered the interaction.
    /// </summary>
    public User User { get; }

    /// <summary>
    /// The continuation token for responding to the interaction.
    /// </summary>
    public string Token => _jsonModel.Token;

    /// <summary>
    /// The interaction version.
    /// </summary>
    public int Version => _jsonModel.Version;

    /// <summary>
    /// The permissions of the application within the context of the interaction.
    /// </summary>
    public Permissions AppPermissions => _jsonModel.AppPermissions;

    /// <summary>
    /// A list of entitlements for the invoking user, representing their active subscriptions or test entitlements.
    /// </summary>
    public IReadOnlyList<Entitlement> Entitlements { get; }

    /// <summary>
    /// The maximum size limit (in bytes) allowed for attachments sent within this interaction context.
    /// </summary>
    public long AttachmentSizeLimit => _jsonModel.AttachmentSizeLimit;

    /// <inheritdoc />
    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) 
        => _sendResponseAsync(this, callback, withResponse, properties, cancellationToken);
}
