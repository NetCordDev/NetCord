using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadUsersUpdateEventArgs(JsonGuildThreadUsersUpdateEventArgs jsonModel, RestClient client)
{
    public ulong ThreadId { get; } = jsonModel.ThreadId;

    public ulong GuildId { get; } = jsonModel.GuildId;

    public int UserCount { get; } = jsonModel.UserCount;

    public IReadOnlyList<AddedThreadUser>? AddedUsers { get; } = CreateAddedUsers(jsonModel, client);

    private static AddedThreadUser[]? CreateAddedUsers(JsonGuildThreadUsersUpdateEventArgs jsonModel, RestClient client)
    {
        if (jsonModel.AddedUsers is { } addedUsers)
        {
            var guildId = jsonModel.GuildId;

            return [.. addedUsers.Select(u => new AddedThreadUser(u, jsonModel.GuildId, client))];
        }

        return null;
    }

    public IReadOnlyList<ulong>? RemovedUserIds { get; } = jsonModel.RemovedUserIds;
}
