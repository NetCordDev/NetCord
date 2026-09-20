using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a gateway ready event, containing initial session configuration and data.
/// </summary>
public class ReadyEventArgs(JsonModels.EventArgs.JsonReadyEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonReadyEventArgs>
{
    JsonModels.EventArgs.JsonReadyEventArgs IJsonModel<JsonModels.EventArgs.JsonReadyEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The gateway API version being used.
    /// </summary>
    public ApiVersion Version => jsonModel.Version;

    /// <summary>
    /// Information about the current bot user.
    /// </summary>
    public CurrentUser User { get; } = new(jsonModel.User, client);

    /// <summary>
    /// A list of IDs for the guilds that the bot is currently in.
    /// </summary>
    public IReadOnlyList<ulong> GuildIds { get; } = jsonModel.Guilds.Select(g => g.Id).ToArray();

    /// <summary>
    /// The unique session identifier used for resuming connections.
    /// </summary>
    public string SessionId => jsonModel.SessionId;

    /// <summary>
    /// The specific gateway URL used for resuming an interrupted session.
    /// </summary>
    public string ResumeGatewayUrl => jsonModel.ResumeGatewayUrl;

    /// <summary>
    /// Information about the current gateway shard, if applicable.
    /// </summary>
    public Shard? Shard => jsonModel.Shard;

    /// <summary>
    /// The unique identifier of the bot application.
    /// </summary>
    public ulong ApplicationId => jsonModel.Application.Id;

    /// <summary>
    /// The configuration and capability flags applied to the bot application.
    /// </summary>
    public ApplicationFlags ApplicationFlags => jsonModel.Application.Flags.GetValueOrDefault();
}
