using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a typing start event, indicating that a user has started typing in a channel.
/// </summary>
public class TypingStartEventArgs(JsonModels.EventArgs.JsonTypingStartEventArgs jsonModel, RestClient client) 
    : IJsonModel<JsonModels.EventArgs.JsonTypingStartEventArgs>
{
    JsonModels.EventArgs.JsonTypingStartEventArgs IJsonModel<JsonModels.EventArgs.JsonTypingStartEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the channel where the user started typing.
    /// </summary>
    public ulong ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The ID of the guild where the user started typing, if applicable.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;

    /// <summary>
    /// The ID of the user who started typing.
    /// </summary>
    public ulong UserId => jsonModel.UserId;

    /// <summary>
    /// The timestamp indicating when the user started typing.
    /// </summary>
    public DateTimeOffset Timestamp => jsonModel.Timestamp;

    /// <summary>
    /// The guild user member object of the user who started typing, if the event occurred in a guild.
    /// </summary>
    public GuildUser? User { get; } = jsonModel.User is { } user 
        ? new GuildUser(user, jsonModel.GuildId.GetValueOrDefault(), client) 
        : null;
}
