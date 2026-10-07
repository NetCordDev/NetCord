using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationActionMetadata(JsonAutoModerationActionMetadata jsonModel)
{
    public ulong? ChannelId { get; } = jsonModel.ChannelId;

    public int? DurationSeconds { get; } = jsonModel.DurationSeconds;

    public string? CustomMessage { get; } = jsonModel.CustomMessage;
}
