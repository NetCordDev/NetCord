using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class Message(JsonMessage jsonModel, Guild? guild, TextChannel? channel, RestClient client) : RestMessage(jsonModel, client), IPartialMessage
{
    public static Message CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var (guild, channel) = IPartialMessage.GetCacheData(jsonModel, cache);
        return new(jsonModel, guild, channel, client);
    }

    public ulong? GuildId => _jsonModel.GuildId;

    public Guild? Guild { get; } = guild;

    public TextChannel? Channel { get; } = channel;

    bool? IPartialMessage.IsTts => IsTts;

    bool? IPartialMessage.MentionEveryone => MentionEveryone;

    bool? IPartialMessage.IsPinned => IsPinned;

    MessageType? IPartialMessage.Type => Type;

    MessageFlags? IPartialMessage.Flags => Flags;
}
