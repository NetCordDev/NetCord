using NetCord.Rest;

namespace NetCord.Gateway;

public class TypingStartEventArgs(JsonModels.EventArgs.JsonTypingStartEventArgs jsonModel, RestClient client)
{
    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public ulong UserId { get; } = jsonModel.UserId;

    public DateTimeOffset Timestamp { get; } = jsonModel.Timestamp;

    public GuildUser? GuildUser { get; } = jsonModel.GuildUser is { } user ? new(user, jsonModel.GuildId.GetValueOrDefault(), client) : null;
}
