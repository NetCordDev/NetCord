using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class MessageReactionAddEventArgs(JsonMessageReactionAddEventArgs jsonModel, RestClient client)
{
    public ulong UserId { get; } = jsonModel.UserId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong MessageId { get; } = jsonModel.MessageId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public GuildUser? GuildUser { get; } = jsonModel.GuildUser is { } user ? new(user, jsonModel.GuildId.GetValueOrDefault(), client) : null;

    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);

    public ulong? MessageAuthorId { get; } = jsonModel.MessageAuthorId;

    public bool Burst { get; } = jsonModel.Burst;

    public IReadOnlyList<Color>? BurstColors { get; } = jsonModel.BurstColors;

    public ReactionType Type { get; } = jsonModel.Type;
}
