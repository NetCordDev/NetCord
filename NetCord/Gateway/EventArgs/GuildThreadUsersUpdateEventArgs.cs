using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a guild thread users update event.
/// </summary>
public class GuildThreadUsersUpdateEventArgs(JsonModels.EventArgs.JsonGuildThreadUsersUpdateEventArgs jsonModel, RestClient client) 
    : IJsonModel<JsonModels.EventArgs.JsonGuildThreadUsersUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildThreadUsersUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildThreadUsersUpdateEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the thread.
    /// </summary>
    public ulong ThreadId => jsonModel.ThreadId;

    /// <summary>
    /// The ID of the guild the thread belongs to.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The approximate number of members in the thread, capped at 50.
    /// </summary>
    public int UserCount => jsonModel.UserCount;

    /// <summary>
    /// The users that were added to the thread, or <see langword="null"/> if no users were added.
    /// </summary>
    public IReadOnlyList<AddedThreadUser>? AddedUsers { get; } = jsonModel.AddedUsers is { } addedUsers 
        ? addedUsers.Select(u => new AddedThreadUser(u, jsonModel.GuildId, client)).ToArray() 
        : null;

    /// <summary>
    /// A list of IDs corresponding to the users that were removed from the thread.
    /// </summary>
    public IReadOnlyList<ulong> RemovedUserIds => jsonModel.RemovedUserIds;
}
