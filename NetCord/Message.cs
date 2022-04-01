using NetCord.JsonModels;

namespace NetCord;

public class Message : RestMessage
{
    internal Message(JsonMessage jsonEntity, GatewayClient client) : base(jsonEntity, client.Rest)
    {
        GuildId = jsonEntity.GuildId ?? jsonEntity.MessageReference?.GuildId;
        if (GuildId.HasValue && client.Guilds.TryGetValue(GuildId.GetValueOrDefault(), out var guild))
        {
            Guild = guild;
            if (guild.Channels.TryGetValue(ChannelId, out var channel))
                Channel = (TextChannel)channel;
            else if (guild.ActiveThreads.TryGetValue(ChannelId, out var thread))
                Channel = thread;
        }
        else
        {
            if (client.DMChannels.TryGetValue(ChannelId, out var dMChannel))
                Channel = dMChannel;
            else if (client.GroupDMChannels.TryGetValue(ChannelId, out var groupDMChannel))
                Channel = groupDMChannel;
        }
    }

    public Snowflake? GuildId { get; }
    public Guild? Guild { get; }
    public TextChannel? Channel { get; }

    public string GetJumpUrl() => $"https://discord.com/channels/{(GuildId.HasValue ? GuildId.GetValueOrDefault() : "@me")}/{ChannelId}/{Id}";
    public override string GetJumpUrl(Snowflake? guildId) => GetJumpUrl();
}