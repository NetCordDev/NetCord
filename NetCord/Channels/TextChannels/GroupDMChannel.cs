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
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The group channel's icon hash.
    /// </summary>
    public string? IconHash => _jsonModel.IconHash;

    /// <summary>
    /// The ID corresponding to the group channel's owner.
    /// </summary>
    public ulong OwnerId => _jsonModel.OwnerId.GetValueOrDefault();

    /// <summary>
    /// The ID corresponding to the application managing the group channel, if any, otherwise <see langword="null"/>.
    /// </summary>
    public ulong? ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Whether the group channel is managed by an application with <see cref="ApplicationFlags.GroupDMCreate"/> set.
    /// </summary>
    public bool Managed => _jsonModel.Managed.GetValueOrDefault();
}
