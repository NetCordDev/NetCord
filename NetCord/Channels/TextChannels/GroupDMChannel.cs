using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a text channel for private messages, with up to 10 users.
/// </summary>
public partial class GroupDMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : DMChannel(jsonModel, client), INamedChannel
{
    /// <summary>
    /// The group channel's name.
    /// </summary>
    public string Name { get; } = jsonModel.Name!;

    /// <summary>
    /// The group channel's icon hash.
    /// </summary>
    public string? IconHash { get; } = jsonModel.IconHash;

    /// <summary>
    /// The ID corresponding to the group channel's owner.
    /// </summary>
    public ulong OwnerId { get; } = jsonModel.OwnerId.GetValueOrDefault();

    /// <summary>
    /// The ID corresponding to the application managing the group channel, if any, otherwise <see langword="null"/>.
    /// </summary>
    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// Whether the group channel is managed by an application with <see cref="ApplicationFlags.GroupDMCreate"/> set.
    /// </summary>
    public bool Managed { get; } = jsonModel.Managed.GetValueOrDefault();
}
