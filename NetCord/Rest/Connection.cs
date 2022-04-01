namespace NetCord;

public class Connection
{
    private readonly JsonModels.JsonConnection _jsonEntity;

    internal Connection(JsonModels.JsonConnection jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Integrations != null)
            Integrations = jsonEntity.Integrations.ToDictionary(i => i.Id, i => new Integration(i, client));
    }

    public string Name => _jsonEntity.Name;

    public ConnectionType Type => _jsonEntity.Type;

    public bool? Revoked => _jsonEntity.Revoked;

    public IReadOnlyDictionary<Snowflake, Integration>? Integrations { get; }

    public bool Verified => _jsonEntity.Verified;

    public bool FriendSync => _jsonEntity.FriendSync;

    public bool ShowActivity => _jsonEntity.ShowActivity;

    public ConnectionVisibility Visibility => _jsonEntity.Visibility;
}