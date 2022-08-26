namespace NetCord.Rest;

public class Integration : Entity, IJsonModel<JsonModels.JsonIntegration>
{
    JsonModels.JsonIntegration IJsonModel<JsonModels.JsonIntegration>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonIntegration _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public IntegrationType Type => _jsonModel.Type;

    public bool Enabled => _jsonModel.Enabled;

    public bool? Syncing => _jsonModel.Syncing;

    public Snowflake? RoleId => _jsonModel.RoleId;

    public bool? EnableEmoticons => _jsonModel.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => _jsonModel.ExpireBehavior;

    public int? ExpireGracePeriod => _jsonModel.ExpireGracePeriod;

    public User? User { get; }

    public Account Account { get; }

    public DateTimeOffset? SyncedAt => _jsonModel.SyncedAt;

    public int? SubscriberCount => _jsonModel.SubscriberCount;

    public bool? Revoked => _jsonModel.Revoked;

    public IntegrationApplication? Application { get; }

    public Integration(JsonModels.JsonIntegration jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.User != null)
            User = new(_jsonModel.User, client);
        Account = new(_jsonModel.Account);

        if (_jsonModel.Application != null)
            Application = new(_jsonModel.Application, client);
    }
}
