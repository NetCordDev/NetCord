using NetCord.JsonModels;

namespace NetCord;

public class UserMessage : Message
{
    internal UserMessage(JsonMessage jsonEntity, BotClient client) : base(jsonEntity, client)
    {
        if (client.TryGetGuild(jsonEntity.GuildId ?? jsonEntity.MessageReference?.GuildId, out var guild))
        {
            Guild = guild;
            Channel = (TextChannel)Guild.GetChannel(jsonEntity.ChannelId);
        }
        else
        {
            Channel = client.GetDMChannel(jsonEntity.ChannelId);
        }
    }

    public Guild? Guild { get; }
    public TextChannel Channel { get; }

    public string GetJumpUrl() => $"https://discord.com/channels/{(Guild != null ? Guild.Id : "@me")}/{ChannelId}/{Id}";
    public override string GetJumpUrl(DiscordId? guildId) => GetJumpUrl();
}