using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class AuthorizationInformation(JsonAuthorizationInformation jsonModel, RestClient client)
{
    /// <summary>
    /// The current application.
    /// </summary>
    public PartialApplication Application { get; } = new(jsonModel.Application, client);

    /// <summary>
    /// The scopes the user has authorized the application for.
    /// </summary>
    public IReadOnlyList<string> Scopes { get; } = jsonModel.Scopes;

    /// <summary>
    /// When the access token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; } = jsonModel.ExpiresAt;

    /// <summary>
    /// The user who has authorized, if the user has authorized with the 'identify' scope.
    /// </summary>
    public User? User { get; } = jsonModel.User is { } user ? new(user, client) : null;
}
