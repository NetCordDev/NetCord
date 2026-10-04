using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildScheduledEventUserEventArgs(JsonGuildScheduledEventUserEventArgs jsonModel)
{
    public ulong GuildScheduledEventId { get; } = jsonModel.GuildScheduledEventId;

    public ulong UserId { get; } = jsonModel.UserId;

    public ulong GuildId { get; } = jsonModel.GuildId;
}
