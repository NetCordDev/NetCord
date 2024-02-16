using NetCord.Rest;

namespace NetCord;

public partial class PrivateGuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : GuildThread(jsonModel, client)
{
}
