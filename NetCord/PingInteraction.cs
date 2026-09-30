using NetCord.Rest;

namespace NetCord;

public class PingInteraction(JsonModels.JsonInteraction jsonModel, InteractionResponseDelegate sendResponseAsync, RestClient client) : Entity, IInteraction
{
    public override ulong Id { get; } = jsonModel.Id;

    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    public User User { get; } = jsonModel.GuildId is { } guildId ? new GuildInteractionUser(jsonModel.GuildUser!, guildId, client) : new User(jsonModel.User!, client);

    public string Token { get; } = jsonModel.Token;

    public int Version { get; } = jsonModel.Version;

    public Permissions AppPermissions { get; } = jsonModel.AppPermissions;

    public IReadOnlyList<Entitlement> Entitlements { get; } = [.. jsonModel.Entitlements.Select(e => new Entitlement(e, client))];

    public long AttachmentSizeLimit { get; } = jsonModel.AttachmentSizeLimit;

    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return sendResponseAsync(this, callback, withResponse, properties, cancellationToken);
    }
}
