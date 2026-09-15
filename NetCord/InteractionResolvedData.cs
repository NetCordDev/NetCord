using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents resolved data contained within an interaction (users, roles, channels, messages, attachments).
/// </summary>
public class InteractionResolvedData(JsonInteractionResolvedData jsonModel, ulong? guildId, RestClient client) : IJsonModel<JsonInteractionResolvedData>
{
    JsonInteractionResolvedData IJsonModel<JsonInteractionResolvedData>.JsonModel => jsonModel;

    /// <summary>
    /// The resolved users and guild members, mapped by their user ID.
    /// </summary>
    public IReadOnlyDictionary<ulong, User>? Users { get; } = jsonModel.Users is { } users
        ? (jsonModel.GuildUsers is { } guildUsers
            ? users.ToDictionary(
                u => u.Key,
                u =>
                {
                    if (guildUsers.TryGetValue(u.Key, out var guildUser))
                    {
                        guildUser.User = u.Value;
                        return (User)new GuildInteractionUser(guildUser, guildId.GetValueOrDefault(), client);
                    }
                    return new User(u.Value, client);
                })
            : users.ToDictionary(u => u.Key, u => new User(u.Value, client)))
        : null;

    /// <summary>
    /// The resolved roles, mapped by their role ID.
    /// </summary>
    public IReadOnlyDictionary<ulong, Role>? Roles { get; } = jsonModel.Roles?.ToDictionary(
        r => r.Key,
        r => new Role(r.Value, guildId.GetValueOrDefault(), client));

    /// <summary>
    /// The resolved channels, mapped by their channel ID.
    /// </summary>
    public IReadOnlyDictionary<ulong, Channel>? Channels { get; } = jsonModel.Channels?.ToDictionary(
        c => c.Key,
        c => Channel.CreateFromJson(c.Value, client));

    /// <summary>
    /// The resolved messages, mapped by their message ID.
    /// </summary>
    public IReadOnlyDictionary<ulong, RestMessage>? Messages { get; } = jsonModel.Messages?.ToDictionary(
        m => m.Key,
        m => new RestMessage(m.Value, client));

    /// <summary>
    /// The resolved attachments, mapped by their attachment ID.
    /// </summary>
    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; } = jsonModel.Attachments?.ToDictionary(
        a => a.Key,
        a => Attachment.CreateFromJson(a.Value, client));
}
