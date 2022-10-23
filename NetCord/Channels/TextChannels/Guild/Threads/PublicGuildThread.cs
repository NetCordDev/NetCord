using NetCord.Rest;

namespace NetCord;

public class PublicGuildThread : GuildThread
{
    public PublicGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public IReadOnlyList<ulong>? AppliedTags => _jsonModel.AppliedTags;
}
