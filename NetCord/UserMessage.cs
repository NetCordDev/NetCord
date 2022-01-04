using NetCord.JsonModels;

namespace NetCord;

public class UserMessage : RestMessage
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal UserMessage(JsonMessage jsonEntity, SocketClient client) : base(jsonEntity, client.Rest)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        DiscordId? guildId = jsonEntity.GuildId ?? jsonEntity.MessageReference?.GuildId;
        if (guildId != null && client.Guilds.TryGetValue(guildId.GetValueOrDefault(), out var guild))
        {
            Guild = guild;
            if (guild._channels.TryGetValue(jsonEntity.ChannelId, out var channel))
                Channel = (TextChannel)channel;
            else if (guild._activeThreads.TryGetValue(jsonEntity.ChannelId, out var thread))
                Channel = thread;
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