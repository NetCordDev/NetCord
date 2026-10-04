using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildBulkBan(JsonGuildBulkBan jsonModel)
{
    public IReadOnlyList<ulong> BannedUsers { get; } = jsonModel.BannedUsers;

    public IReadOnlyList<ulong> FailedUsers { get; } = jsonModel.FailedUsers;
}
