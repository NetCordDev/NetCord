using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a connection account connected to a Discord account.
/// </summary>
public class Connection(JsonConnection jsonModel, RestClient client) : IJsonModel<JsonConnection>
{
    JsonConnection IJsonModel<JsonConnection>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the connection account.
    /// </summary>
    public string Id => jsonModel.Id;

    /// <summary>
    /// The username of the connection account.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The service of this connection.
    /// </summary>
    public ConnectionType Type => jsonModel.Type;

    /// <summary>
    /// Whether the connection is revoked.
    /// </summary>
    public bool? Revoked => jsonModel.Revoked;

    /// <summary>
    /// A list of server integrations.
    /// </summary>
    public IReadOnlyList<Integration>? Integrations { get; } = jsonModel.Integrations?.Select(i => new Integration(i, client)).ToArray();

    /// <summary>
    /// Whether the connection is verified.
    /// </summary>
    public bool Verified => jsonModel.Verified;

    /// <summary>
    /// Whether friend sync is enabled for this connection.
    /// </summary>
    public bool FriendSync => jsonModel.FriendSync;

    /// <summary>
    /// Whether activities related to this connection will be shown in presence updates.
    /// </summary>
    public bool ShowActivity => jsonModel.ShowActivity;

    /// <summary>
    /// Whether this connection has a corresponding third party OAuth2 token.
    /// </summary>
    public bool TwoWayLink => jsonModel.TwoWayLink;

    /// <summary>
    /// Visibility of this connection.
    /// </summary>
    public ConnectionVisibility Visibility => jsonModel.Visibility;
}
