using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a complete <see cref="Message"/> object, with all required fields present.
/// </summary>
public class Message(JsonMessage jsonModel, Guild? guild, TextChannel? channel, RestClient client) : RestMessage(jsonModel, client), IPartialMessage
{
    public static Message CreateFromJson(JsonMessage jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var (guild, channel) = IPartialMessage.GetCacheData(jsonModel, cache);
        return new(jsonModel, guild, channel, client);
    }

    /// <inheritdoc/>
    public ulong? GuildId => _jsonModel.GuildId;

    /// <inheritdoc/>
    public Guild? Guild { get; } = guild;

    /// <inheritdoc/>
    public TextChannel? Channel { get; } = channel;

    /// <inheritdoc/>
    bool? IPartialMessage.IsTts => IsTts;

    /// <inheritdoc/>
    bool? IPartialMessage.MentionEveryone => MentionEveryone;

    /// <inheritdoc/>
    bool? IPartialMessage.IsPinned => IsPinned;

    /// <inheritdoc/>
    MessageType? IPartialMessage.Type => Type;

    /// <inheritdoc/>
    MessageFlags? IPartialMessage.Flags => Flags;
}
