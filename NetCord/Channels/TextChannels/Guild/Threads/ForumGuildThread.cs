using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class ForumGuildThread(JsonChannel jsonModel, RestClient client) : PublicGuildThread(jsonModel, client)
{
    public RestMessage Message { get; } = new(jsonModel.Message!, client);
}
