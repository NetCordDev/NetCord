using NetCord.JsonModels;

namespace NetCord;

public class UserMessage : Message
{
    internal UserMessage(JsonMessage jsonEntity, BotClient client) : base(jsonEntity, client)
    {
        DiscordId? guildId = jsonEntity.GuildId ?? jsonEntity.MessageReference?.GuildId;
        if (guildId != null && client.Guilds.TryGetValue(guildId, out var guild))
        {
            Guild = guild;
            if (Guild.Channels.TryGetValue(jsonEntity.ChannelId, out var channel))
                Channel = (TextChannel)channel;
        }
        else
        {
            if (client.DMChannels.TryGetValue(jsonEntity.ChannelId, out var dMChannel))
                Channel = dMChannel;
            else if (client.GroupDMChannels.TryGetValue(jsonEntity.ChannelId, out var groupDMChannel))
                Channel = groupDMChannel;
        }
    }

    public Guild? Guild { get; }
    public TextChannel Channel { get; }

    public string GetJumpUrl() => $"https://discord.com/channels/{(Guild != null ? Guild.Id : "@me")}/{ChannelId}/{Id}";
    public override string GetJumpUrl(DiscordId? guildId) => GetJumpUrl();
}