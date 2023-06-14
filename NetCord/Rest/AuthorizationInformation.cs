namespace NetCord.Rest;

public class AuthorizationInformation : IJsonModel<JsonModels.JsonAuthorizationInformation>
{
    JsonModels.JsonAuthorizationInformation IJsonModel<JsonModels.JsonAuthorizationInformation>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonAuthorizationInformation _jsonModel;

    public AuthorizationInformation(JsonModels.JsonAuthorizationInformation jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Application = new(jsonModel.Application, client);

        var user = jsonModel.User;
        if (user is not null)
            User = new(user, client);
    }

    /// <summary>
    /// The current application.
    /// </summary>
    public Application Application { get; }

    /// <summary>
    /// The scopes the user has authorized the application for.
    /// </summary>
    public IReadOnlyList<string> Scopes => _jsonModel.Scopes;

    /// <summary>
    /// When the access token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt => _jsonModel.ExpiresAt;

    /// <summary>
    /// The user who has authorized, if the user has authorized with the 'identify' scope.
    /// </summary>
    public User? User { get; }
}
