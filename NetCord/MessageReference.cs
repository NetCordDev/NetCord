using NetCord.JsonModels;

namespace NetCord;

public class MessageReference(JsonMessageReference jsonModel)
{
    public MessageReferenceType Type { get; } = jsonModel.Type.GetValueOrDefault();

    public ulong MessageId { get; } = jsonModel.MessageId.GetValueOrDefault();

    public ulong ChannelId { get; } = jsonModel.ChannelId.GetValueOrDefault();

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public bool? FailIfNotExists { get; } = jsonModel.FailIfNotExists;
}
