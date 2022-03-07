namespace NetCord;

public class Integration : Entity
{
    private readonly JsonModels.JsonIntegration _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public IntegrationType Type => _jsonEntity.Type;

    public bool Enabled => _jsonEntity.Enabled;

    public bool? Syncing => _jsonEntity.Syncing;

    public DiscordId? RoleId => _jsonEntity.RoleId;

    public bool? EnableEmoticons => _jsonEntity.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => _jsonEntity.ExpireBehavior;

    public int? ExpireGracePeriod => _jsonEntity.ExpireGracePeriod;

    public User? User { get; }

    public Account Account { get; }

    public DateTimeOffset? SyncedAt => _jsonEntity.SyncedAt;

    public int? SubscriberCount => _jsonEntity.SubscriberCount;

    public bool? Revoked => _jsonEntity.Revoked;

    public IntegrationApplication? Application { get; }

    internal Integration(JsonModels.JsonIntegration jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (_jsonEntity.User != null)
            User = new(_jsonEntity.User, client);
        Account = new(_jsonEntity.Account);

        if (_jsonEntity.Application != null)
            Application = new(_jsonEntity.Application, client);
    }
}