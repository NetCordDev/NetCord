using NetCord.Rest;

namespace NetCord;

public partial class PublicGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
    public IReadOnlyList<ulong>? AppliedTags => _jsonModel.AppliedTags;
}
