namespace NetCord;

public class IntegrationApplication : Entity
{
    private readonly JsonModels.JsonIntegrationApplication _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public string? IconHash => _jsonEntity.IconHash;

    public string Description => _jsonEntity.Description;

    public string Summary => _jsonEntity.Summary;

    public User? Bot { get; }

    internal IntegrationApplication(JsonModels.JsonIntegrationApplication jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (_jsonEntity.Bot != null)
            Bot = new(_jsonEntity.Bot, client);
    }
}