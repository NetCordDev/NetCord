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

    public static Message CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        Guild? guild;
        TextChannel? channel;
        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
        {
            if (cache.Guilds.TryGetValue(guildId.GetValueOrDefault(), out guild))
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
            if (cache.DMChannels.TryGetValue(channelId, out var dMChannel))
                channel = dMChannel;
            else if (cache.GroupDMChannels.TryGetValue(channelId, out var groupDMChannel))
                channel = groupDMChannel;
            else
                channel = null;
        }
        return new(jsonModel, guild, channel, client);
    }

    public ulong? GuildId => _jsonModel.GuildId;
    public Guild? Guild { get; }
    public TextChannel? Channel { get; }
}
