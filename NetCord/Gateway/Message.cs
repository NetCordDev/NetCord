using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a complete <see cref="Message"/> object, with all required fields present.
/// </summary>
public class Message(JsonMessage jsonModel, Guild? guild, TextChannel? channel, RestClient client) : RestMessage(jsonModel, client)
{
    public static Message CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var (guild, channel) = GetCacheData(jsonModel, cache);
        return new(jsonModel, guild, channel, client);
    }

    private static (Guild?, TextChannel?) GetCacheData(JsonMessage jsonModel, IGatewayClientCache cache)
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
            channel = null;
        }

        return (guild, channel);
    }

    /// <inheritdoc/>
    public ulong? GuildId => _jsonModel.GuildId;

    /// <inheritdoc/>
    public Guild? Guild { get; } = guild;

    /// <inheritdoc/>
    public TextChannel? Channel { get; } = channel;
}
