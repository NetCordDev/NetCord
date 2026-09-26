using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Contains an interaction's resolved information.
/// </summary>
public class InteractionResolvedData
{
    /// <summary>
    /// A list of user objects, mapped to their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, User>? Users { get; }

    /// <summary>
    /// A list of role objects, mapped to their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, Role>? Roles { get; }

    /// <summary>
    /// A list of channel objects, mapped to their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, Channel>? Channels { get; }

    /// <summary>
    /// A list of message objects, mapped to their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, RestMessage>? Messages { get; }

    /// <summary>
    /// A list of attachment objects, mapped to their IDs.
    /// </summary>
    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; }

    public InteractionResolvedData(JsonInteractionResolvedData jsonModel, ulong? guildId, RestClient client)
    {
        if (jsonModel.Users is { } users)
        {
            if (jsonModel.GuildUsers is { } guildUsers)
            {
                var guildIdValue = guildId.GetValueOrDefault();

                Users = users.ToDictionary(u => u.Key, u =>
                {
                    if (guildUsers.TryGetValue(u.Key, out var guildUser))
                    {
                        guildUser.User = u.Value;
                        return new GuildInteractionUser(guildUser, guildIdValue, client);
                    }
                    else
                        return new User(u.Value, client);
                });
            }
            else
                Users = users.ToDictionary(u => u.Key, u => new User(u.Value, client));
        }

        if (jsonModel.Roles is { } roles)
            Roles = roles.ToDictionary(r => r.Key, r => new Role(r.Value, guildId.GetValueOrDefault(), client));

        if (jsonModel.Channels is { } channels)
            Channels = channels.ToDictionary(c => c.Key, c => Channel.CreateFromJson(c.Value, client));

        if (jsonModel.Messages is { } messages)
            Messages = messages.ToDictionary(m => m.Key, m => new RestMessage(m.Value, client));

        if (jsonModel.Attachments is { } attachments)
            Attachments = attachments.ToDictionary(c => c.Key, c => Attachment.CreateFromJson(c.Value, client));
    }
}
