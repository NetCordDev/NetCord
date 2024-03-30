using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildBulkBan(JsonGuildBulkBan jsonModel) : IJsonModel<JsonGuildBulkBan>
{
    JsonGuildBulkBan IJsonModel<JsonGuildBulkBan>.JsonModel => jsonModel;

    public IReadOnlyList<ulong> BannedUsers => jsonModel.BannedUsers;

    public IReadOnlyList<ulong> FailedUsers => jsonModel.FailedUsers;
}
