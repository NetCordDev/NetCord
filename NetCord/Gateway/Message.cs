using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a complete <see cref="Message"/> object, with all required fields present.
/// </summary>
public class Message(JsonMessage jsonModel, Guild? guild, TextGuildChannel? channel, RestClient client) : RestMessage(jsonModel, client)
{
    public static Message CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var (guild, channel) = GetCacheData(jsonModel, cache);
        return new(jsonModel, guild, channel, client);
    }

    private static (Guild?, TextGuildChannel?) GetCacheData(JsonMessage jsonModel, IGatewayClientCache cache)
    {
        Guild? guild;
        TextGuildChannel? channel;
        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
        {
            if (cache.Guilds.TryGetValue(guildId.GetValueOrDefault(), out guild))
            {
                var channelId = jsonModel.ChannelId;
                if (guild.Channels.TryGetValue(channelId, out var guildChannel))
                    channel = guildChannel as TextGuildChannel;
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

    /// <summary>
    /// The ID of the guild the message was sent in.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> when the message is ephemeral or sent outside of a guild.
    /// </remarks>
    public ulong? GuildId { get; } = jsonModel.GuildId;

    /// <summary>
    /// The type of channel the message was sent in.
    /// </summary>
    public ChannelType? ChannelType { get; } = jsonModel.ChannelType;

    /// <summary>
    /// The guild the message was sent in.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> when the message is ephemeral or sent outside of a guild, or if the guild is not cached.
    /// </remarks>
    public Guild? Guild { get; } = guild;

    /// <summary>
    /// The channel the message was sent in.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> when the message is ephemeral or sent outside of a guild, or if the channel is not cached.
    /// </remarks>
    public TextGuildChannel? Channel { get; } = channel;
}
