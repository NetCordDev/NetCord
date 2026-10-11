using NetCord.Rest;

namespace NetCord;

public class Integration(JsonModels.JsonIntegration jsonModel, RestClient client) : Entity(jsonModel)
{
    public string Name { get; } = jsonModel.Name;

    public IntegrationType Type { get; } = jsonModel.Type;

    public bool Enabled { get; } = jsonModel.Enabled;

    public bool? Syncing { get; } = jsonModel.Syncing;

    public ulong? RoleId { get; } = jsonModel.RoleId;

    public bool? EnableEmoticons { get; } = jsonModel.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior { get; } = jsonModel.ExpireBehavior;

    public int? ExpireGracePeriod { get; } = jsonModel.ExpireGracePeriod;

    public User? User { get; } = jsonModel.User is { } user ? new(user, client) : null;

    public IntegrationAccount Account { get; } = new(jsonModel.Account);

    public DateTimeOffset? SyncedAt { get; } = jsonModel.SyncedAt;

    public int? SubscriberCount { get; } = jsonModel.SubscriberCount;

    public bool? Revoked { get; } = jsonModel.Revoked;

    public Application? Application { get; } = jsonModel.Application is { } application ? new(application, client) : null;

    public IReadOnlyList<string>? Scopes { get; } = jsonModel.Scopes;
}
