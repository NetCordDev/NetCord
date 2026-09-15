using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a message reaction add event.
/// </summary>
public class MessageReactionAddEventArgs(JsonModels.EventArgs.JsonMessageReactionAddEventArgs jsonModel, RestClient client) 
    : IJsonModel<JsonModels.EventArgs.JsonMessageReactionAddEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionAddEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionAddEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the user who added the reaction.
    /// </summary>
    public ulong UserId => jsonModel.UserId;

    /// <summary>
    /// The ID of the channel where the message resides.
    /// </summary>
    public ulong ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// The ID of the message that received the reaction.
    /// </summary>
    public ulong MessageId => jsonModel.MessageId;

    /// <summary>
    /// The ID of the guild where the reaction was added, if applicable.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;

    /// <summary>
    /// The guild user member object of the user who added the reaction, if the reaction was added in a guild.
    /// </summary>
    public GuildUser? User { get; } = jsonModel.User is { } user 
        ? new GuildUser(user, jsonModel.GuildId.GetValueOrDefault(), client) 
        : null;

    /// <summary>
    /// The emoji that was used for the reaction.
    /// </summary>
    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);

    /// <summary>
    /// The ID of the author of the message that received the reaction, if available.
    /// </summary>
    public ulong? MessageAuthorId => jsonModel.MessageAuthorId;

    /// <summary>
    /// Whether the reaction was added as a super reaction (burst reaction).
    /// </summary>
    public bool Burst => jsonModel.Burst;

    /// <summary>
    /// A list of colors applied to the super reaction animation, if applicable.
    /// </summary>
    public IReadOnlyList<Color> BurstColors => jsonModel.BurstColors;

    /// <summary>
    /// The type of the reaction that was added.
    /// </summary>
    public ReactionType Type => jsonModel.Type;
}
