using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a guild widget.
/// </summary>
public class GuildWidget(JsonGuildWidget jsonModel, RestClient client) : Entity, IJsonModel<JsonGuildWidget>
{
    JsonGuildWidget IJsonModel<JsonGuildWidget>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The name of the guild.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The instant invite code for the guild.
    /// </summary>
    public string? InstantInvite => jsonModel.InstantInvite;

    /// <summary>
    /// The channels in the guild widget.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildWidgetChannel> Channels { get; } = jsonModel.Channels.ToDictionary(c => c.Id, c => new GuildWidgetChannel(c));

    /// <summary>
    /// The online users in the guild widget.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> Users { get; } = jsonModel.Users.ToDictionary(u => u.Id, u => new User(u, client));

    /// <summary>
    /// The number of online members in the guild widget.
    /// </summary>
    public int PresenceCount => jsonModel.PresenceCount;
}
