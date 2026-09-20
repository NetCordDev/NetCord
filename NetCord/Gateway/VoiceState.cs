using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a user's voice connection state.
/// </summary>
public class VoiceState(JsonModels.JsonVoiceState jsonModel, ulong guildId, RestClient client) : IJsonModel<JsonModels.JsonVoiceState>
{
    JsonModels.JsonVoiceState IJsonModel<JsonModels.JsonVoiceState>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the guild this voice state is associated with.
    /// </summary>
    public ulong GuildId { get; } = guildId;

    /// <summary>
    /// The ID of the channel the user is connected to, or <see langword="null"/> if disconnected.
    /// </summary>
    public ulong? ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The ID of the user this voice state is for.
    /// </summary>
    public ulong UserId => jsonModel.UserId;

    /// <summary>
    /// The guild member this voice state is for.
    /// </summary>
    public GuildUser? User { get; } = jsonModel.User is { } user ? new(user, jsonModel.GuildId.GetValueOrDefault(), client) : null;

    /// <summary>
    /// The session ID for this voice state.
    /// </summary>
    public string SessionId => jsonModel.SessionId;

    /// <summary>
    /// Whether this user is deafened by the server.
    /// </summary>
    public bool IsDeafened => jsonModel.IsDeafened;

    /// <summary>
    /// Whether this user is muted by the server.
    /// </summary>
    public bool IsMuted => jsonModel.IsMuted;

    /// <summary>
    /// Whether this user is locally deafened.
    /// </summary>
    public bool IsSelfDeafened => jsonModel.IsSelfDeafened;

    /// <summary>
    /// Whether this user is locally muted.
    /// </summary>
    public bool IsSelfMuted => jsonModel.IsSelfMuted;

    /// <summary>
    /// Whether this user is streaming using "Go Live".
    /// </summary>
    public bool? SelfStreamExists => jsonModel.SelfStreamExists;

    /// <summary>
    /// Whether this user's camera is enabled.
    /// </summary>
    public bool SelfVideoExists => jsonModel.SelfVideoExists;

    /// <summary>
    /// Whether this user is muted by the current user (suppressed).
    /// </summary>
    public bool Suppressed => jsonModel.Suppressed;

    /// <summary>
    /// The time at which the user requested to speak in a Stage channel.
    /// </summary>
    public DateTimeOffset? RequestToSpeakTimestamp => jsonModel.RequestToSpeakTimestamp;
}
