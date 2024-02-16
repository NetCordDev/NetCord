using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class Message(JsonMessage jsonModel, Guild? guild, TextChannel? channel, RestClient client) : RestMessage(jsonModel, client)
{
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
            channel = cache.DMChannels.GetValueOrDefault(jsonModel.ChannelId);
        }
        return new(jsonModel, guild, channel, client);
    }

    public ulong? GuildId => _jsonModel.GuildId;
    public Guild? Guild { get; } = guild;
    public TextChannel? Channel { get; } = channel;
}
