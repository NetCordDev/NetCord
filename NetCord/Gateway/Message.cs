using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class Message : RestMessage
{
    public Message(JsonMessage jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, client)
    {
        Guild = guild;
        Channel = channel;
    }

    public static Message CreateFromJson(JsonMessage jsonModel, GatewayClient client)
    {
        Guild? guild;
        TextChannel? channel;
        var guildId = jsonModel.GuildId ?? jsonModel.MessageReference?.GuildId;
        if (guildId.HasValue)
        {
            if (client.Guilds.TryGetValue(guildId.GetValueOrDefault(), out guild))
            {
                var channelId = jsonModel.ChannelId;
                if (guild.Channels.TryGetValue(channelId, out var guildChannel))
                    channel = (TextChannel)guildChannel;
                else if (guild.ActiveThreads.TryGetValue(channelId, out var thread))
                    channel = thread;
                else
                    channel = null;
            }
            else
                channel = null;
        }
        else
        {
            guild = null;
            var channelId = jsonModel.ChannelId;
            if (client.DMChannels.TryGetValue(channelId, out var dMChannel))
                channel = dMChannel;
            else if (client.GroupDMChannels.TryGetValue(channelId, out var groupDMChannel))
                channel = groupDMChannel;
            else
                channel = null;
        }
        return new(jsonModel, guild, channel, client.Rest);
    }

    public Guild? Guild { get; }
    public TextChannel? Channel { get; }
}
