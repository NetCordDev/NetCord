using NetCord.JsonModels;

namespace NetCord;

public class GuildWelcomeScreenChannel(JsonGuildWelcomeScreenChannel jsonModel) : Entity(jsonModel.ChannelId)
{
    public string Description { get; } = jsonModel.Description;

    public ulong? EmojiId { get; } = jsonModel.EmojiId;

    public string? EmojiName { get; } = jsonModel.EmojiName;
}
