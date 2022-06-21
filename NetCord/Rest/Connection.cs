namespace NetCord.Rest;

public class Connection : IJsonModel<JsonModels.JsonConnection>
{
    JsonModels.JsonConnection IJsonModel<JsonModels.JsonConnection>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonConnection _jsonModel;

    public Connection(JsonModels.JsonConnection jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Integrations != null)
            Integrations = jsonModel.Integrations.ToDictionary(i => i.Id, i => new Integration(i, client));
    }

    public string Name => _jsonModel.Name;

    public ConnectionType Type => _jsonModel.Type;

    public bool? Revoked => _jsonModel.Revoked;

    public IReadOnlyDictionary<Snowflake, Integration>? Integrations { get; }

    public bool Verified => _jsonModel.Verified;

    public bool FriendSync => _jsonModel.FriendSync;

    public bool ShowActivity => _jsonModel.ShowActivity;

    public ConnectionVisibility Visibility => _jsonModel.Visibility;
}