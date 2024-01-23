using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class ForumGuildThread : PublicGuildThread
{
    public ForumGuildThread(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Message = new(jsonModel.Message!, client);
    }

    public RestMessage Message { get; }
}
