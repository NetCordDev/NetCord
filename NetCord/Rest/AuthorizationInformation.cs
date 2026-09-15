using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents OAuth2 authorization information.
/// </summary>
public class AuthorizationInformation(JsonAuthorizationInformation jsonModel, RestClient client) : IJsonModel<JsonAuthorizationInformation>
{
    JsonAuthorizationInformation IJsonModel<JsonAuthorizationInformation>.JsonModel => jsonModel;

    /// <summary>
    /// The current application.
    /// </summary>
    public Application Application { get; } = new(jsonModel.Application, client);

    /// <summary>
    /// The scopes the user has authorized the application for.
    /// </summary>
    public IReadOnlyList<string> Scopes => jsonModel.Scopes;

    /// <summary>
    /// When the access token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt => jsonModel.ExpiresAt;

    /// <summary>
    /// The user who has authorized, if the user has authorized with the <c>identify</c> scope.
    /// </summary>
    public User? User { get; } = jsonModel.User is { } user ? new(user, client) : null;
}
