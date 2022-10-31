using NetCord.JsonModels;

namespace NetCord.Rest;

public class AuthorizationInformation : IJsonModel<JsonAuthorizationInformation>
{
    JsonAuthorizationInformation IJsonModel<JsonAuthorizationInformation>.JsonModel => _jsonModel;
    private readonly JsonAuthorizationInformation _jsonModel;

    public AuthorizationInformation(JsonAuthorizationInformation jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Application = new(jsonModel.Application, client);
        if (jsonModel.User != null)
            User = new(jsonModel.User, client);
    }

    public Application Application { get; }

    public IReadOnlyList<string> Scopes => _jsonModel.Scopes;

    public DateTimeOffset ExpiresAt => _jsonModel.ExpiresAt;

    public User? User { get; }
}
